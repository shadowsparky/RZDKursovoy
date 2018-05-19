/*MySQL scripts for DB*/

DROP PROCEDURE IF EXISTS rzd.FindPassengerWithPersonalData;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.FindPassengerWithPersonalData(IN Passport_Series_IN INT, IN Passport_Number_IN INT, KeySi TEXT)
BEGIN
  SELECT Last_Name, First_Name, Pathronymic, AES_DECRYPT(Passenger_Phone_Number, KeySi)
  FROM passengers
  WHERE Passport_Series = AES_ENCRYPT(Passport_Series_IN, KeySi) AND Passport_Number = AES_ENCRYPT(Passport_Number_IN, KeySi);
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.ThrowAllStations;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.ThrowAllStations()
BEGIN
  SELECT DISTINCT Stop_Name FROM stops;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.Available_Railcar_Types;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.Available_Railcar_Types(
	IN `Train_Number_IN` VARCHAR(11)
)
BEGIN
  SELECT DISTINCT Railcar_Type, Cost FROM railcars
  INNER JOIN railcar_types ON railcars.Railcar_Type_Number = railcar_types.Railcar_Type_Number
  WHERE Train_Number = Train_Number_IN and railcar_types.Seats_Count <> 0 AND railcar_types.Cost <> 0;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.TrainInfoTwo;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.TrainInfoTwo(IN TrainNumber VARCHAR(10), IN Arrival_ID_IN INT, IN DepartureID_IN INT)
BEGIN
	DECLARE ArrivalTime time;
  DECLARE ArrivalDate Date;
  DECLARE StopDeparture int;
  DECLARE StopArrival int;
 	DECLARE DepartureTime time;
	DECLARE DepartureDate Date;
	DECLARE FirstStation text;
	DECLARE LastStation_ID int;
	DECLARE LastStation text;
	DECLARE RoutID int;
  DECLARE RailwayStation varchar(50);

  SELECT arrivals.Rout_ID INTO RoutID FROM arrivals
  WHERE (Arrival_ID = Arrival_ID_IN);

  SELECT Stop_ID INTO StopDeparture FROM arrivals
  WHERE Arrival_ID = Arrival_ID_IN;

  SELECT Departure_Time INTO DepartureTime FROM departures
  WHERE Train_Number = TrainNumber and Rout_ID = RoutID and Stop_ID = StopDeparture;

  SELECT Stop_ID INTO StopArrival FROM departures
  WHERE Departure_ID = DepartureID_IN;

  SELECT Arrival_Date INTO ArrivalDate FROM arrivals
  WHERE Train_Number = TrainNumber and Rout_ID = RoutID and Stop_ID = StopArrival;

  SELECT Arrival_Time INTO ArrivalTime FROM arrivals
  WHERE Train_Number = TrainNumber and Rout_ID = RoutID and Stop_ID = StopArrival;

  SELECT Arrivals.Stop_ID INTO LastStation_ID FROM Arrivals
  WHERE (Train_Number = TrainNumber)
  ORDER BY Stop_ID DESC LIMIT 1;

  SELECT Stop_Name INTO FirstStation FROM stops
  INNER JOIN arrivals ON arrivals.Stop_ID = stops.Stop_ID
  WHERE (stops.Rout_ID = RoutID) AND (arrivals.Arrival_ID = Arrival_ID_IN);

  SELECT Stop_Name INTO LastStation FROM stops
  INNER JOIN departures ON departures.Stop_ID = stops.Stop_ID
  WHERE (stops.Rout_ID = RoutID) AND (departures.Departure_ID = DepartureID_IN);

  SELECT Train_Station_Name INTO RailwayStation FROM stops
  WHERE Stop_ID = StopDeparture and Rout_ID = RoutID;

  IF (RailwayStation is NOT NULL) THEN
    SET FirstStation = CONCAT(FirstStation, ', ', RailwayStation);
  END IF;

  SELECT Train_Station_Name INTO RailwayStation FROM stops
  WHERE Stop_ID = StopArrival and Rout_ID = RoutID;

  IF (RailwayStation is NOT NULL) THEN
    SET LastStation = CONCAT(LastStation, ', ', RailwayStation);
  END IF;
  SELECT TrainNumber, TIME_FORMAT(DepartureTime, '%H:%i'), TIME_FORMAT(ArrivalTime, '%H:%i'), DATE_FORMAT(ArrivalDate, '%d.%m.%Y'),
  FirstStation, LastStation;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.FindRout;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.FindRout(in ArrivalStation_IN varchar(30),in DepartureStation_IN varchar(30),in Arrival_Date_IN varchar(30))
BEGIN
	DECLARE LastID int;
	DECLARE ArrivalID int;
	DECLARE DepartureID int;
	DECLARE i int;
	DECLARE TrainID int;
	DECLARE TMPRout int;
	DECLARE TMPKostil int;
	DECLARE TrueDate date;
  DECLARE Arrival_Stop_ID int;

	DROP TABLE IF EXISTS TMPResultTable;
	CREATE TEMPORARY TABLE TMPResultTable(RoutID_ int);
	SET trueDate = STR_TO_DATE(Arrival_Date_IN, '%d.%m.%Y');
	SET i = 1;

  SELECT Rout_ID INTO LastID FROM routes
  ORDER BY Rout_ID DESC LIMIT 1;

  SET LastID = LastID + 1;
	WHILE (i <> LastID) DO
    SELECT stops.Rout_ID INTO TMPRout FROM Stops
    INNER JOIN arrivals ON arrivals.Rout_ID = stops.Rout_ID
    WHERE (stops.Rout_ID = i) AND (stops.Stop_Name = ArrivalStation_IN) AND (arrivals.Arrival_Date = TrueDate) LIMIT 1;
		IF (TMPRout is NOT NULL) THEN
      SELECT Rout_ID INTO TMPKostil FROM Stops
      WHERE (Rout_ID = TMPRout) AND (Stop_Name = DepartureStation_IN);
			IF (TMPKostil is NOT NULL) THEN
        INSERT INTO TMPResultTable (RoutID_) VALUES (TMPKostil);
      END IF;
    END IF;
	  SET i = i + 1;
  END WHILE;
  SELECT DISTINCT * FROM TMPResultTable;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.ThrowTrainNumbersList;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.ThrowTrainNumbersList(IN Train_Number_IN VARCHAR(11), IN Railcar_Type_IN VARCHAR(30))
BEGIN
  SELECT DISTINCT Railcar_Number FROM railcars
  INNER JOIN railcar_types ON railcars.Railcar_Type_Number = railcar_types.Railcar_Type_Number
  WHERE Train_Number = Train_Number_IN AND Railcar_Type = Railcar_Type_IN;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.Available_For_Planting_Seats;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.Available_For_Planting_Seats(IN TrainNumber_IN VARCHAR(11), IN Railcar_Number_IN INT, IN CurrentArrivalID INT, IN CurrentDepartureID INT)
BEGIN
	DECLARE MaxCount int;
	DECLARE i int;
	DECLARE i2 int;
	DECLARE TMPPlaceID int;
	DECLARE CurrentStopArrivalID int;
	DECLARE CurrentStopDepartureID int;
	DECLARE ExistsStopArrivalID int;
	DECLARE ExistsStopDepartureID int;
	DECLARE MaxCompareCount int;
	DECLARE Error int;
  DECLARE TMPResult int;

	DROP TABLE IF EXISTS TMPFreePlaces;
	CREATE TEMPORARY table TMPFreePlaces(
		`Place_ID` int AUTO_INCREMENT,
		`Place_Number` int,
		PRIMARY KEY (`Place_ID`)
	);

  SELECT ThrowArrivalStopID(CurrentArrivalID) INTO CurrentStopArrivalID;
  SELECT ThrowDepartureStopID(CurrentDepartureID) INTO CurrentStopDepartureID;
  SET i = 1;
  SET i2 = 1;
  SELECT railcar_types.Seats_Count INTO MaxCount FROM railcar_types
  INNER JOIN railcars ON railcars.Railcar_Type_Number = railcar_types.Railcar_Type_Number
  WHERE (railcars.Train_Number = TrainNumber_IN) AND (railcars.Railcar_Number = Railcar_Number_IN);

  WHILE (i <> MaxCount + 1) DO
		SET ERROR = 1;
		SET TMPPlaceID = NULL;
		SET i2 = 1;
		SELECT Place_ID INTO TMPPlaceID FROM places
		WHERE (Train_Number = TrainNumber_IN) AND (Railcar_Number = Railcar_Number_IN) AND (Place_Number = i);
  	IF (TMPPlaceID is NULL) then
			INSERT INTO TMPFreePlaces (Place_Number) VALUES (i);
		ELSE
  		DROP table IF exists TMPComparetable;
  		CREATE TEMPORARY table TMPComparetable(
  			`CompareNumber` int AUTO_INCREMENT,
  			`Arrival_Stop_ID` int,
  			`Departure_Stop_ID` int,
  			PRIMARY KEY (`CompareNumber`)
  			);
  	   INSERT INTO TMPComparetable (Arrival_Stop_ID, Departure_Stop_ID)
       SELECT arrivals.Stop_ID, departures.Stop_ID FROM reservation
       INNER JOIN departures ON reservation.Departure_ID = departures.Departure_ID
       INNER JOIN arrivals ON reservation.Arrival_ID = arrivals.Arrival_ID
       WHERE reservation.Place_Number = TMPPlaceID;
  	   SELECT COUNT(*) INTO MaxCompareCount FROM TMPComparetable;
  	   WHILE (i2 <> MaxCompareCount + 1) do
  		   SELECT Arrival_Stop_ID INTO ExistsStopArrivalID FROM TMPComparetable
  		   WHERE CompareNumber = i2;
  		   SELECT Departure_Stop_ID INTO ExistsStopDepartureID FROM TMPComparetable
  		   WHERE CompareNumber = i2;
  		   SELECT ThrowEmployedIDs(CurrentStopArrivalID, CurrentStopDepartureID, ExistsStopArrivalID, ExistsStopDepartureID) INTO TMPResult;
  		   IF (TMPResult = -1) THEN
  		     set Error = 0;
  		   END IF;
  		  set i2 = i2 + 1;
  		  IF (Error = 0) THEN
  			  set i2 = MaxCompareCount + 1;
  		  end IF;
  	   END WHILE;
  	   IF (Error = 1) THEN
  		    INSERT INTO TMPFreePlaces (Place_Number) VALUES (i);
  	   END IF;
    END IF;
    SET i = i + 1;
  END WHILE;
  SELECT Place_Number FROM TMPFreePlaces;
END$$
DELIMITER;

CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.newFindTrainList(
	`Rout_ID_IN` int,
	`Arrival_Stop_Name` varchar(30),
	`Arrival_Date_IN` varchar(30),
  `Departure_Stop_Name` varchar(30)
)
BEGIN
	DECLARE TrainNumber int;
	DECLARE CurrentStopID int;
  DECLARE CurrentDepartureStopID int;
  DECLARE CheckDate Date;
  DECLARE trueDate Date;
  SET CheckDate = now();
  SET trueDate = STR_TO_DATE(Arrival_Date_IN, '%d.%m.%Y');
  IF (trueDate > CheckDate) THEN
    SELECT Stop_ID INTO CurrentStopID FROM stops
    WHERE (Rout_ID = Rout_ID_IN) AND (Stop_Name = Arrival_Stop_Name);
    SELECT Stop_ID INTO CurrentDepartureStopID FROM stops
    WHERE (Rout_ID = Rout_ID_IN) AND (Stop_Name = Departure_Stop_Name);
    if (CurrentStopID < CurrentDepartureStopID) AND (CurrentStopID <> CurrentDepartureStopID) then
      SELECT Train_Number FROM arrivals
      WHERE (Arrival_Date = trueDate) AND (Stop_ID = CurrentStopID) AND (Rout_ID = Rout_ID_IN);
    ELSE
      SELECT -1;
    END IF;
  ELSE
    SELECT -1;
  END IF;
END

DROP PROCEDURE IF EXISTS rzd.throwPassengerInfo;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.throwPassengerInfo(IN Reservation_ID_IN INT, IN KeySi TEXT)
BEGIN
  DECLARE PassNum int;
  SELECT Passenger_ID INTO PassNum FROM reservation
  WHERE Reservation_ID = Reservation_ID_IN;
  SELECT Last_Name, First_Name, Pathronymic, AES_DECRYPT(Passport_Series, KeySi), AES_DECRYPT(Passport_Number, KeySi) FROM passengers
  WHERE Passenger_ID = PassNum;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.GetDepartureID;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION rzd.GetDepartureID(DepartureStopName_IN VARCHAR(30), DepartureRoutID_IN INT, TrainNumber_IN VARCHAR(30))
  RETURNS int(11)
BEGIN
  DECLARE TMPID int;
  SELECT departures.Departure_ID INTO TMPID
  FROM departures
  INNER JOIN stops ON departures.Stop_ID = stops.Stop_ID AND departures.Rout_ID = stops.Rout_ID
  WHERE stops.Stop_Name = DepartureStopName_IN AND departures.Rout_ID = DepartureRoutID_IN AND departures.Train_Number = TrainNumber_IN;
	RETURN TMPID;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.GetArrivalID;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION rzd.GetArrivalID(ArrivalStopName_IN VARCHAR(30), ArrivalRoutID_IN INT, TrainNumber_IN VARCHAR(30))
  RETURNS int(11)
BEGIN
	DECLARE TMPID int;
  SELECT Arrivals.Arrival_ID INTO TMPID
  FROM arrivals
  INNER JOIN stops ON arrivals.Stop_ID = stops.Stop_ID AND arrivals.Rout_ID = stops.Rout_ID
  WHERE stops.Stop_Name = ArrivalStopName_IN AND arrivals.Rout_ID = ArrivalRoutID_IN AND arrivals.Train_Number = TrainNumber_IN;
	RETURN TMPID;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.FindPassenger;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION rzd.FindPassenger(
	`Passport_Series_IN` int,
	`Passport_Number_IN` int,
  `KeySi` TEXT
)
  RETURNS int(11)
BEGIN
	DECLARE tmpid int;
  SELECT Passenger_ID INTO tmpid
  FROM passengers
  WHERE Passport_Series = AES_ENCRYPT(Passport_Series_IN, KeySi) AND Passport_Number = AES_ENCRYPT(Passport_Number_IN, KeySi);
	IF (tmpid is NOT NULL) THEN
		RETURN tmpid;
	ELSE
		RETURN -1;
	END IF;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.PassengerAddToDB;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION rzd.PassengerAddToDB(Last_Name_IN VARCHAR(30), First_Name_IN VARCHAR(30), Pathronymic_IN VARCHAR(30), Passport_Series_IN INT(11), Passport_Number_IN INT(11), Passenger_Phone_Number_IN VARCHAR(20), KeySi text)
  RETURNS int(11)
BEGIN
  DECLARE LastInsertID int;
  INSERT INTO passengers (Last_Name, First_Name, Pathronymic, Passport_Series, Passport_Number, Passenger_Phone_Number)
  VALUES (Last_Name_IN, First_Name_IN, Pathronymic_IN, AES_ENCRYPT(Passport_Series_IN, KeySi), AES_ENCRYPT(Passport_Number_IN, KeySi), AES_ENCRYPT(Passenger_Phone_Number_IN, KeySi));
  SET LastInsertID = last_insert_id();
  RETURN LastInsertID;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.ThrowRoutID;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION rzd.ThrowRoutID(Train_Number_IN VARCHAR(30))
  RETURNS int(11)
BEGIN
  DECLARE CurrentRoutID int;
  SELECT Rout_ID INTO CurrentRoutID FROM trains
  WHERE Train_Number = Train_Number_IN;
  IF (CurrentRoutID is NOT NULL) THEN
    RETURN CurrentRoutID;
  ELSE
    RETURN -1;
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.throwCountAvailableTickets;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.throwCountAvailableTickets(IN UserLogin_IN VARCHAR(30))
BEGIN
  SELECT COUNT(*) FROM reservation
  WHERE Payed_Account_Login = UserLogin_IN;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.CancelTicket;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.CancelTicket(IN TicketNumber INT)
BEGIN
  DELETE FROM reservation
  WHERE Reservation_ID = TicketNumber;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.throwAvailableTicketsWithInfo;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.throwAvailableTicketsWithInfo(IN User_Login_IN VARCHAR(30))
BEGIN
  -- TMP Vars
  DECLARE _TMPReservationID int;
  DECLARE _TMPPlaceNumber int;
  DECLARE _TMPDepartureID int;
  DECLARE _TMPArrivalID int;
  -- Other Vars
  DECLARE _Train_Number VARCHAR(11);
  DECLARE _Railcar_Number int;
  DECLARE StopDeparture int;
  DECLARE StopArrival int;
  DECLARE _Place_Number int;
  DECLARE _Arrival_Date Date;
  DECLARE _Arrival_Time Time;
  DECLARE _Arrival_Stop_Name varchar(30);
  DECLARE _Arrival_Railway_Station varchar(30);
  DECLARE _Arrival_Rout_ID int;
  DECLARE _Departure_Date Date;
  DECLARE _Departure_Time Time;
  DECLARE _Departure_Stop_Name varchar(30);
  DECLARE _Departure_Railway_Station varchar(30);
  -- Cursor Vars
  DECLARE done INT DEFAULT 0;
  DECLARE cur1 CURSOR FOR
    SELECT Reservation_ID, Place_Number, Departure_ID, Arrival_ID FROM reservation
    WHERE Payed_Account_Login = User_Login_IN;
  DECLARE CONTINUE HANDLER FOR SQLSTATE '02000' SET done = 1;
  -- Code
  OPEN cur1;

	DROP TABLE IF EXISTS ResultTable;
  CREATE TEMPORARY table ResultTable(
    `Reservation_ID` int,
	  `Train_Number` VARCHAR(11),
	  `Railcar_Number` int,
    `Place_Number` int,
    `Arrival_Time` time,
    `Arrival_Date` date,
    `Arrival_Stop_Name` varchar(30),
    `Departure_Time` time,
    `Departure_Date` date,
    `Departure_Stop_Name` varchar(30)
  );

  WHILE (done <> 1) DO
    FETCH cur1 INTO _TMPReservationID,_TMPPlaceNumber, _TMPDepartureID, _TMPArrivalID;
    SELECT Train_Number INTO _Train_Number FROM places
    WHERE Place_ID = _TMPPlaceNumber;
    SELECT Railcar_Number INTO _Railcar_Number FROM places
    WHERE Place_ID = _TMPPlaceNumber;
    SELECT Place_Number INTO _Place_Number FROM places
    WHERE Place_ID = _TMPPlaceNumber;

    SELECT Rout_ID INTO _Arrival_Rout_ID FROM arrivals
    WHERE Arrival_ID = _TMPArrivalID;

    SELECT Stop_ID INTO StopDeparture FROM arrivals
    WHERE Arrival_ID = _TMPArrivalID;

    SELECT Stop_ID INTO StopArrival FROM departures
    WHERE Departure_ID = _TMPDepartureID;

    SELECT Arrival_Date INTO _Arrival_Date FROM arrivals
    WHERE Train_Number = _Train_Number and Rout_ID = _Arrival_Rout_ID and Stop_ID = StopArrival;

    SELECT Arrival_Time INTO _Arrival_Time FROM arrivals
    WHERE Train_Number = _Train_Number and Rout_ID = _Arrival_Rout_ID and Stop_ID = StopArrival;

    SELECT stops.Stop_Name INTO _Arrival_Stop_Name FROM arrivals
    INNER JOIN stops ON arrivals.Stop_ID = stops.Stop_ID
    WHERE arrivals.Arrival_ID = _TMPArrivalID AND stops.Rout_ID = _Arrival_Rout_ID;

    SELECT Train_Station_Name INTO _Arrival_Railway_Station FROM stops
    WHERE Stop_ID = StopDeparture and Rout_ID = _Arrival_Rout_ID;

    IF (_Arrival_Railway_Station IS NOT NULL) THEN
      SET _Arrival_Stop_Name = CONCAT(_Arrival_Stop_Name, ', ', _Arrival_Railway_Station);
    END IF;

    SELECT Departure_Time INTO _Departure_Time FROM departures
    WHERE Train_Number = _Train_Number and Rout_ID = _Arrival_Rout_ID and Stop_ID = StopDeparture;

    SELECT Departure_Date INTO _Departure_Date FROM departures
    WHERE Train_Number = _Train_Number and Rout_ID = _Arrival_Rout_ID and Stop_ID = StopDeparture;

    SELECT stops.Stop_Name INTO _Departure_Stop_Name FROM departures
    INNER JOIN stops ON departures.Stop_ID = stops.Stop_ID
    WHERE departures.Departure_ID = _TMPDepartureID AND stops.Rout_ID = _Arrival_Rout_ID;

    SELECT Train_Station_Name INTO _Departure_Railway_Station FROM stops
    WHERE Stop_ID = StopArrival and Rout_ID = _Arrival_Rout_ID;

    IF (_Departure_Railway_Station IS NOT NULL) THEN
      SET _Departure_Stop_Name = CONCAT(_Departure_Stop_Name, ', ', _Departure_Railway_Station);
    END IF;
    INSERT INTO ResultTable (Reservation_ID, Train_Number, Railcar_Number, Place_Number, Arrival_Time, Arrival_Date, Arrival_Stop_Name, Departure_Time, Departure_Date, Departure_Stop_Name)
    VALUES (_TMPReservationID, _Train_Number, _Railcar_Number, _Place_Number, _Arrival_Time, _Arrival_Date, _Arrival_Stop_Name, _Departure_Time, _Departure_Date, _Departure_Stop_Name);
  END WHILE;

  SELECT DISTINCT Reservation_ID, Train_Number, Railcar_Number, Place_Number,
  TIME_FORMAT(Departure_Time, '%H:%i'), DATE_FORMAT(Departure_Date, '%d.%m.%Y'),
  Arrival_Stop_Name, TIME_FORMAT(Arrival_Time, '%H:%i'), DATE_FORMAT(Arrival_Date, '%d.%m.%Y'),
  Departure_Stop_Name FROM ResultTable;
  DROP TABLE IF EXISTS ResultTable;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.EmployPlaces;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.EmployPlaces(
	IN `Train_Number_IN` VARCHAR(11),
	IN `Railcar_Number_IN` int,
	IN `Place_Number_IN` int,
	IN `Passenger_ID_IN` int,
	IN `Arrival_ID_IN` int,
	IN `Departure_ID_IN` int
)
begin
	DECLARE CurrentDepartureID int;
	DECLARE CurrentStopArrivalID int;
	DECLARE CurrentStopDepartureID int;
	DECLARE ExistsStopDepartureID int;
	declare TmpDepartureID int;
	declare CurrentPlaceID int;
	DECLARE CompareTableCount int;
	DECLARE i int;
	DECLARE Reservation int;
	DECLARE MaxCompareCount int;
  DECLARE EA int;
  DECLARE ED int;
  DECLARE CurrentUserLogin varchar(30);
	SET i = 1;
	SET Reservation = 1;
  SELECT Place_ID INTO CurrentPlaceID FROM places
  WHERE ((Train_Number = Train_Number_IN) AND (Railcar_Number = Railcar_Number_IN) AND (Place_Number = Place_Number_IN));
  SELECT Stop_ID INTO CurrentStopArrivalID FROM arrivals
  WHERE (Arrival_ID = Arrival_ID_IN);
  SELECT Stop_ID INTO CurrentStopDepartureID FROM departures
  WHERE (Departure_ID = Departure_ID_IN);
  if (CurrentStopArrivalID < CurrentStopDepartureID) AND (CurrentStopArrivalID <> CurrentStopDepartureID) then
		if (CurrentPlaceID is NULL) then
      INSERT INTO Places (Train_Number, Railcar_Number, Place_Number) VALUES (Train_Number_IN, Railcar_Number_IN, Place_Number_IN);
 		  SET CurrentPlaceID = LAST_INSERT_ID();
      SELECT USER() INTO CurrentUserLogin;
      INSERT INTO Reservation (Passenger_ID, Place_Number, Departure_ID, Arrival_ID, Payed_Account_Login) VALUES (Passenger_ID_IN, CurrentPlaceID, Departure_ID_IN, Arrival_ID_IN, CurrentUserLogin);
    ELSE
		  DROP TABLE IF EXISTS TMPCompareTable;
			CREATE TEMPORARY TABLE TMPCompareTable(
				`CompareNumber` int AUTO_INCREMENT,
				`Arrival_Stop_ID` int,
				`Departure_Stop_ID` int,
				PRIMARY KEY (`CompareNumber`)
			);
      INSERT INTO TMPCompareTable (Arrival_Stop_ID, Departure_Stop_ID)
      SELECT arrivals.Stop_ID, departures.Stop_ID
      FROM reservation
      INNER JOIN departures ON reservation.Departure_ID = departures.Departure_ID
      INNER JOIN arrivals ON reservation.Arrival_ID = arrivals.Arrival_ID
      WHERE reservation.Place_Number = CurrentPlaceID;
      SELECT COUNT(*) INTO MaxCompareCount
      FROM TMPCompareTable;
			WHILE (i <> MaxCompareCount + 1) do
        SELECT Arrival_Stop_ID INTO EA FROM TMPCompareTable
        WHERE CompareNumber = i;
        SELECT Departure_Stop_ID INTO ED FROM TMPCompareTable
        WHERE CompareNumber = i;
        IF (EA <> NULL) and (ED <> NULL) THEN
  			  IF (ThrowEmployedIDs(CurrentStopArrivalID, CurrentStopDepartureID, EA, ED) = -1) THEN
            SET Reservation = 0;
  			  END IF;
          SET i = i + 1;
          IF (Reservation = 0) THEN
            SET i = MaxCompareCount + 1;
          END IF;
         ELSE
           SET i = i + 1;
         END IF;
			END WHILE;
			if (Reservation = 0) then
        signal sqlstate '45000' SET message_text = 'К сожалению, выбранное вами место уже занято';
      ELSE
        SELECT USER() INTO CurrentUserLogin;
        INSERT INTO Reservation (Passenger_ID, Place_Number, Departure_ID, Arrival_ID, Payed_Account_Login) VALUES (Passenger_ID_IN, CurrentPlaceID, Departure_ID_IN, Arrival_ID_IN, CurrentUserLogin);
      END IF;
    END IF;
  ELSE
		signal sqlstate '45000' SET message_text = 'Произошла техническая ошибка, обратитесь к администратору';
	end if;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.register_createuser;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.register_createuser(IN UserLogin varchar(30), IN UserPassword varchar(30))
BEGIN
	SET @query1 = CONCAT('CREATE USER "',UserLogin,'"@"localhost" IDENTIFIED BY "',UserPassword,'" ');
	PREPARE stmt FROM @query1;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
	SET @query2 = CONCAT('grant user to "', UserLogin,'"@"localhost"');
	PREPARE stmt FROM @query2;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
	SET @query3 = CONCAT('set default role user for "', UserLogin,'"@"localhost"');
	PREPARE stmt FROM @query3;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
	FLUSH PRIVILEGES;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.throwRailcarInfo;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.throwRailcarInfo(IN Train_Number_IN VARCHAR(11), IN Railcar_Number_IN INT)
BEGIN
  SELECT railcar_types.Railcar_Type, railcar_types.Cost FROM railcars
  INNER JOIN railcar_types ON railcar_types.Railcar_Type_Number = railcars.Railcar_Type_Number
  WHERE Train_Number = Train_Number_IN AND Railcar_Number = Railcar_Number_IN;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.EmptyProcForBlockedUsers;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.EmptyProcForBlockedUsers()
BEGIN

END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.ThrowArrivalStopID;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION ThrowArrivalStopID(Arrival_ID_IN int)
  RETURNS int(11)
BEGIN
	DECLARE CurrentStopID int;
  SELECT Stop_ID INTO CurrentStopID FROM arrivals
  WHERE (Arrival_ID = Arrival_ID_IN);
	IF (CurrentStopID is NOT NULL) THEN
	 	RETURN CurrentStopID;
	ELSE
		signal sqlstate '45000' SET message_text = 'Во время запроса произошла ошибка. Виноват в этом только программист.';
	END IF;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.ThrowDepartureStopID;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION ThrowDepartureStopID(Departure_ID_IN int)
  RETURNS int(11)
BEGIN
	DECLARE CurrentStopID int;
  SELECT Stop_ID INTO CurrentStopID FROM departures
  WHERE (Departure_ID = Departure_ID_IN);
	IF (CurrentStopID is NOT NULL) THEN
		RETURN CurrentStopID;
	else
		signal sqlstate '45000' SET message_text = 'Во время запроса произошла ошибка. Виноват в этом только программист.';
	END IF;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.ThrowEmployedIDs;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION ThrowEmployedIDs(CA INT, CD INT, EA INT, ED INT)
  RETURNS int(11)
BEGIN
  IF (((CA <= ED) AND (CD <= EA)) OR ((CA >= ED) AND (CD >= ED))) THEN
    RETURN 1;
  ELSE
    RETURN -1;
  END IF;
END$$
DELIMITER;

DROP TRIGGER IF EXISTS rzd.RemovePhantomSeats;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
TRIGGER RemovePhantomSeats
	AFTER DELETE ON reservation
	FOR EACH ROW
BEGIN
  DECLARE CurrentCount int;
  SELECT COUNT(*) INTO CurrentCount FROM reservation
  WHERE Place_Number = old.Place_Number;
  if (CurrentCount = 0) then
    DELETE FROM places
    WHERE Place_ID = old.Place_Number;
  END IF;
END$$
DELIMITER ;

/*Dispatcher Scripts*/
DROP PROCEDURE IF EXISTS rzd.DISPATCHER_AddTrain;

DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_AddTrain(IN Train_Number_IN varchar(30), IN Railcar_Count_IN int, IN Train_Type_IN varchar(30), IN Rout_Name_IN varchar(50))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID INT;
  DECLARE _TMPNum INT;
  IF (Railcar_Count_IN < 20) THEN
    SELECT Rout_ID INTO _TMPRoutID FROM routes
    WHERE Rout_Name = Rout_Name_IN LIMIT 1;
    SELECT Train_Type_Number INTO _TMPNum FROM train_types
    WHERE Train_Type = Train_Type_IN;
    IF (_TMPNum is NULL) THEN
      INSERT INTO train_types (Train_Type) VALUES (Train_Type_IN);
      SET _TMPNum = LAST_INSERT_ID();
    END IF;
    IF (_TMPRoutID is NOT NULL) then
      INSERT INTO trains (Train_Number, Railcar_Count, Train_Type_Number, Rout_ID)
      VALUES (Train_Number_IN, Railcar_Count_IN, _TMPNum, Rout_ID_IN);
    ELSE
      signal sqlstate '45000' SET message_text = 'Такого маршрута не существует. Перепроверьте введенные данные';
    END IF;
  ELSE
    signal sqlstate '45000' SET message_text = 'Вы ввели слишком большое количество вагонов. Перепроверьте введенные данные';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_ShowTrains;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_ShowTrains()
  DETERMINISTIC
BEGIN
  SELECT Train_Number, Railcar_Count, train_types.Train_Type, routes.Rout_Name FROM trains
  INNER JOIN train_types ON trains.Train_Type_Number = train_types.Train_Type_Number
  INNER JOIN routes ON trains.Rout_ID = routes.Rout_ID;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_DropTrain;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_DropTrain(IN Train_Number_IN varchar(30))
  DETERMINISTIC
BEGIN
  DECLARE EXIT HANDLER FOR SQLSTATE '23000'
  BEGIN
    signal sqlstate '45000' SET message_text = 'При удалении поезда произошла ошибка. Убедитесь, что у удаляемого поезда нет записей в таблицах "Отправления" и "Прибытия"';
  END;
  DELETE FROM trains WHERE Train_Number = Train_Number_IN;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_UpdateTrain;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_UpdateTrain(IN Train_Number_IN varchar(30), IN Railcar_Count_IN int, IN Train_Type_IN varchar(30), IN Rout_Name_IN varchar(50))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID INT;
  DECLARE _TMPNum INT;
  IF (Railcar_Count_IN < 20) THEN
    SELECT Rout_ID INTO _TMPRoutID FROM routes
    WHERE Rout_Name = Rout_Name_IN LIMIT 1;
    SELECT Train_Type_Number INTO _TMPNum FROM train_types
    WHERE Train_Type = Train_Type_IN;
    IF (_TMPNum is NULL) THEN
      INSERT INTO train_types (Train_Type) VALUES (Train_Type_IN);
      SET _TMPNum = LAST_INSERT_ID();
    END IF;
    IF (_TMPRoutID is NOT NULL) THEN
      UPDATE trains SET Railcar_Count = Railcar_Count_IN, Train_Type_Number = _TMPNum, Rout_ID = _TMPRoutID
      WHERE Train_Number = Train_Number_IN;
    ELSE
      signal sqlstate '45000' SET message_text = 'Такого маршрута не существует. Перепроверьте введенные данные';
    END IF;
  ELSE
    signal sqlstate '45000' SET message_text = 'Вы ввели слишком большое количество вагонов. Перепроверьте введенные данные';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_AddRailcar;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_AddRailcar(IN Train_Number_IN varchar(30), IN Railcar_Number_IN int, IN Railcar_Type_IN varchar(30))
  DETERMINISTIC
BEGIN
  DECLARE _TMPNum int;
  DECLARE _MaxRailcars_Count int;
  SELECT Railcar_Type_Number INTO _TMPNum FROM railcar_types
  WHERE Railcar_Type = Railcar_Type_IN;
  INSERT INTO railcars (Train_Number, Railcar_Number, Railcar_Type_Number) VALUES (Train_Number_IN, Railcar_Number_IN, _TMPNum);
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_ShowRailcars;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_ShowRailcars()
  DETERMINISTIC
BEGIN
  SELECT Train_Number, Railcar_Number, railcar_types.Railcar_Type FROM railcars
  INNER JOIN railcar_types ON railcars.Railcar_Type_Number = railcar_types.Railcar_Type_Number;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_DropRailcar;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_DropRailcar(IN Train_Number_IN VARCHAR(30), IN Railcar_Number_IN int)
  DETERMINISTIC
BEGIN
  DELETE FROM railcars WHERE Train_Number = Train_Number_IN AND Railcar_Number = Railcar_Number_IN;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_UpdateRailcar;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_UpdateRailcar(IN Train_Number_IN VARCHAR(30), IN Railcar_Number_IN int, IN Railcar_Type_IN varchar(30))
  DETERMINISTIC
BEGIN
  DECLARE _TMPNum int;
  DECLARE _MaxRailcars_Count int;
  DECLARE _Error text;
  SELECT Railcar_Type_Number INTO _TMPNum FROM railcar_types
  WHERE Railcar_Type = Railcar_Type_IN;
  IF (_TMPNum is NOT NULL) THEN
    SELECT Railcar_Count INTO _MaxRailcars_Count FROM trains
    WHERE Train_Number = Train_Number_IN;
    SET _Error = CONCAT('У данного поезда всего ', _MaxRailcars_Count, ' вагонов.');
    IF (_MaxRailcars_Count > Railcar_Number_IN) then
      SIGNAL SQLSTATE '45000' SET message_text = _Error;
    ELSE
      UPDATE railcars SET Railcar_Number = Railcar_Number_IN, Railcar_Type_Number = _TMPNum;
    END IF;
  ELSE
    SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Введенного вами типа вагона не существует';
  END IF;
END$$
DELIMITER;

DROP FUNCTION IF EXISTS rzd.DISPATCHER_AddRout;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
FUNCTION rzd.DISPATCHER_AddRout(Stop_Count_IN int, Rout_Name_IN varchar(50))
  RETURNS int(11)
BEGIN
  DECLARE TMPNum int;
  INSERT INTO routes (Stop_Count, Rout_Name) VALUES (Stop_Count_IN, Rout_Name_IN);
  SET TMPNum = LAST_INSERT_ID();
  RETURN TMPNum;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_DropRout;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_DropRout(IN Rout_ID_IN INT)
  DETERMINISTIC
BEGIN
  DELETE FROM routes WHERE Rout_ID = Rout_ID_IN;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_UpdateRout;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_UpdateRout(IN Rout_ID_IN INT, IN Stop_Count_IN int, IN Rout_Name_IN varchar(50))
  DETERMINISTIC
BEGIN
  UPDATE routes SET Stop_Count = Stop_Count_IN, Rout_Name = Rout_Name_IN
  WHERE Rout_ID = Rout_ID_IN;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_ShowRoutes;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_ShowRoutes()
  DETERMINISTIC
BEGIN
  SELECT Rout_ID, Stop_Count, Rout_Name FROM routes;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_AddStop;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_AddStop(IN Rout_Name_IN varchar(50), IN Stop_ID_IN int, IN Stop_Name_IN varchar(30), IN Train_Station_Name_IN varchar(50))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPMaxStopCount int;
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SELECT Stop_Count INTO _TMPMaxStopCount FROM routes
  WHERE Rout_ID = _TMPRoutID;
  IF (_TMPRoutID is NOT NULL) THEN
    IF (_TMPMaxStopCount > Stop_ID_IN) THEN
      INSERT INTO stops (Rout_ID, Stop_ID, Stop_Name, Train_Station_Name) VALUES (_TMPRoutID, Stop_ID_IN, Stop_Name_IN, Train_Station_Name_IN);
    END IF;
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowStopNumber;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowStopNumber(IN Rout_Name_IN varchar(50))
  DETERMINISTIC
BEGIN
  DECLARE MaxCount int;
  DECLARE i int;
  DROP TABLE IF EXISTS TMPStopListCount;
  CREATE TEMPORARY table TMPStopListCount(
    `Stop_ID` int AUTO_INCREMENT,
    `Stop_Number` int,
    PRIMARY KEY (`Stop_ID`)
  );
  SELECT Stop_Count INTO MaxCount FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SET i = 1;
  WHILE (i <> Stop_Count + 1) DO
    INSERT INTO TMPStopListCount (Stop_Number) VALUES (i);
    SET i = i + 1;
  END WHILE;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_UpdateStop;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_UpdateStop(IN Rout_Name_IN varchar(50), IN Stop_ID_IN int, IN Stop_Name_IN varchar(30), IN Train_Station_Name_IN varchar(50))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPMaxStopCount int;
  DECLARE _error TEXT;
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  IF (_TMPRoutID is not null) THEN
    SELECT Stop_Count INTO _TMPMaxStopCount FROM routes
    WHERE Rout_ID = _TMPRoutID;
      IF (_TMPMaxStopCount > Stop_ID_IN) THEN
        IF (Train_Station_Name_IN = '') THEN
          UPDATE stops SET Stop_Name = Stop_Name_IN, Train_Station_Name = NULL
          WHERE Rout_ID = _TMPRoutID and Stop_ID = Stop_ID_IN;
        ELSE
          UPDATE stops SET Stop_Name = Stop_Name_IN, Train_Station_Name = Train_Station_Name_IN
          WHERE Rout_ID = _TMPRoutID and Stop_ID = Stop_ID_IN;
        END IF;
      ELSE
        SET _error = CONCAT('При обновлении записи произошла ошибка. В выбранном маршруте всего ', _TMPMaxStopCount, ' остановок');
        SIGNAL SQLSTATE '45000' SET message_text = _error;
      END IF;
  ELSE
    SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Введенного вами маршрута не существует';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_DropStop;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_DropStop(IN Rout_Name_IN varchar(50), IN Stop_ID_IN int)
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPMaxStopCount int;
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  IF (_TMPRoutID is NOT NULL) THEN
    DELETE FROM stops WHERE Rout_ID = _TMPRoutID AND Stop_ID = Stop_ID_IN;
  ELSE
    SIGNAL SQLSTATE '45000' SET message_text = 'При удалении записи произошла невозможная ошибка';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_ShowStops;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_ShowStops()
  DETERMINISTIC
BEGIN
  SELECT routes.Rout_Name, Stop_ID, Stop_Name, Train_Station_Name FROM stops
  INNER JOIN routes ON stops.Rout_ID = routes.Rout_ID;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_AddArrival;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_AddArrival(IN Rout_Name_IN varchar(30), IN Stop_Name_IN varchar(30), IN Train_Number_IN varchar(11), IN Arrival_Time_IN time, IN Arrival_Date_IN varchar(11))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPStopID int;
  SET trueDate = STR_TO_DATE(Arrival_Date_IN, '%d.%m.%Y');
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SELECT Stop_ID INTO _TMPStopID FROM Stops
  WHERE Rout_ID = _TMPRoutID AND Stop_Name = Stop_Name_IN;
  IF (_TMPStopID is NOT NULL) THEN
      INSERT INTO arrivals (Stop_ID, Rout_ID, Train_Number, Arrival_Time, Arrival_Date) VALUES (_TMPStopID, _TMPRoutID, Train_Number_IN, Arrival_Time_IN, TrueDate);
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowAvailableStopsFindByRout;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowAvailableStopsFindByRout(IN Rout_Name_IN varchar(30))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SELECT Stop_Name FROM stops
  WHERE Rout_ID = _TMPRoutID;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowAvailableTrainsFindByRout;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowAvailableTrainsFindByRout(IN Rout_Name_IN varchar(30))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SELECT Train_Number FROM trains
  WHERE Rout_ID = _TMPRoutID;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_UpdateArrival;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_UpdateArrival(IN Rout_Name_IN varchar(30), IN Stop_Name_IN varchar(30), IN Train_Number_IN varchar(11), IN Arrival_Time_IN time, IN Arrival_Date_IN varchar(11))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPStopID int;
  DECLARE TrueDate date;
  SET trueDate = STR_TO_DATE(Arrival_Date_IN, '%d.%m.%Y');
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  IF EXISTS(SELECT * FROM trains WHERE Train_Number = Train_Number_IN) THEN
    IF (_TMPRoutID is NOT NULL) THEN
      SELECT Stop_ID INTO _TMPStopID FROM Stops
      WHERE Rout_ID = _TMPRoutID AND Stop_Name = Stop_Name_IN;
      IF (_TMPStopID is NOT NULL) THEN
        UPDATE arrivals SET Train_Number = Train_Number_IN, Arrival_Time = Arrival_Time_IN, Arrival_Date = trueDate
        WHERE Stop_ID = _TMPStopID AND Rout_ID = _TMPRoutID;
      ELSE
        SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Введенной вами остановки не существует';
      END IF;
    ELSE
      SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Введенного вами маршрута не существует';
    END IF;
  ELSE
    SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Поезда с введеным вами номером не существует';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_DropArrival;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_DropArrival(IN Rout_Name_IN varchar(30), IN Stop_Name_IN varchar(30), IN Train_Number_IN varchar(11))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPStopID int;
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SELECT Stop_ID INTO _TMPStopID FROM Stops
  WHERE Rout_ID = _TMPRoutID AND Stop_Name = Stop_Name_IN;
  IF (_TMPStopID is NOT NULL) THEN
    DELETE FROM arrivals WHERE Stop_ID = _TMPStopID AND Rout_ID = _TMPRoutID;
  ELSE
    SIGNAL SQLSTATE '45000' SET message_text = 'При удалении записи произошла неизвестная ошибка';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_ShowArrivals;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_ShowArrivals()
  DETERMINISTIC
BEGIN
  SELECT stops.Stop_Name, routes.Rout_Name, Train_Number, TIME_FORMAT(Arrival_Time, '%H:%i'), DATE_FORMAT(Arrival_Date, '%d.%m.%Y') FROM arrivals
  INNER JOIN stops ON arrivals.Stop_ID = stops.Stop_ID AND arrivals.Rout_ID = stops.Rout_ID
  INNER JOIN routes ON arrivals.Rout_ID = routes.Rout_ID;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_AddDeparture;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_AddDeparture(IN Rout_Name_IN varchar(30), IN Stop_Name_IN varchar(30), IN Train_Number_IN varchar(11), IN Departure_Time_IN time, IN Departure_Date_IN varchar(11))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPStopID int;
  DECLARE trueDate date;
  SET trueDate = STR_TO_DATE(Arrival_Date_IN, '%d.%m.%Y');
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SELECT Stop_ID INTO _TMPStopID FROM Stops
  WHERE Rout_ID = _TMPRoutID AND Stop_Name = Stop_Name_IN;
  IF (_TMPStopID is NOT NULL) THEN
      INSERT INTO departures (Stop_ID, Rout_ID, Train_Number, Departure_Time, Departure_Date) VALUES (_TMPStopID, _TMPRoutID, Train_Number_IN, Departure_Time_IN, trueDate);
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_UpdateDeparture;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_UpdateDeparture(IN Stop_Name_IN varchar(30), IN Rout_Name_IN varchar(30), IN Train_Number_IN varchar(11), IN Departure_Time_IN time, IN Departure_Date_IN varchar(11))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPStopID int;
  DECLARE TrueDate date;
  SET trueDate = STR_TO_DATE(Departure_Date_IN, '%d.%m.%Y');
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  IF EXISTS(SELECT * FROM trains WHERE Train_Number = Train_Number_IN) THEN
    IF (_TMPRoutID is NOT NULL) THEN
      SELECT Stop_ID INTO _TMPStopID FROM Stops
      WHERE Rout_ID = _TMPRoutID AND Stop_Name = Stop_Name_IN;
      IF (_TMPStopID is NOT NULL) THEN
        UPDATE departures SET Train_Number = Train_Number_IN, Departure_Time = Departure_Time_IN, Departure_Date = trueDate
        WHERE Stop_ID = _TMPStopID AND Rout_ID = _TMPRoutID;
      ELSE
        SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Введенной вами остановки не существует';
      END IF;
    ELSE
      SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Введенного вами маршрута не существует';
    END IF;
  ELSE
    SIGNAL SQLSTATE '45000' SET message_text = 'При обновлении записи произошла ошибка. Поезда с введеным вами номером не существует';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_DropDeparture;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_DropDeparture(IN Rout_Name_IN varchar(30), IN Stop_Name_IN varchar(30), IN Train_Number_IN varchar(11))
  DETERMINISTIC
BEGIN
  DECLARE _TMPRoutID int;
  DECLARE _TMPStopID int;
  SELECT Rout_ID INTO _TMPRoutID FROM routes
  WHERE Rout_Name = Rout_Name_IN;
  SELECT Stop_ID INTO _TMPStopID FROM Stops
  WHERE Rout_ID = _TMPRoutID AND Stop_Name = Stop_Name_IN;
  IF (_TMPStopID is NOT NULL) THEN
    DELETE FROM departures WHERE Stop_ID = _TMPStopID AND Rout_ID = _TMPRoutID;
  ELSE
    SIGNAL SQLSTATE '45000' SET message_text = 'При удалении записи произошла неизвестная ошибка';
  END IF;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.DISPATCHER_ShowDepartures;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.DISPATCHER_ShowDepartures()
  DETERMINISTIC
BEGIN
  SELECT stops.Stop_Name, routes.Rout_Name, Train_Number, TIME_FORMAT(Departure_Time, '%H:%i'), DATE_FORMAT(Departure_Date, '%d.%m.%Y') FROM departures
  INNER JOIN stops ON departures.Stop_ID = stops.Stop_ID AND departures.Rout_ID = stops.Rout_ID
  INNER JOIN routes ON departures.Rout_ID = routes.Rout_ID;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowRailcarTypesList;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowRailcarTypesList()
BEGIN
  SELECT DISTINCT Railcar_Type FROM railcar_types;
END$$
DELIMITER

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowTrainList;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowTrainList()
BEGIN
  SELECT DISTINCT Train_Number FROM trains;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowAvailableRailcarList;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowAvailableRailcarList(IN Train_Number_IN VARCHAR(11))
BEGIN
  DECLARE i int;
  DECLARE MaxRailcarsCount int;
  SET i = 1;
  DROP TABLE IF EXISTS TMPRailcarListCount;
	CREATE TEMPORARY table TMPRailcarListCount(
		`RailcarCount_ID` int AUTO_INCREMENT,
		`Railcar` int,
		PRIMARY KEY (`RailcarCount_ID`)
	);
  SELECT Railcar_Count INTO MaxRailcarsCount FROM trains
  WHERE Train_Number = Train_Number_IN;
  WHILE (i <> MaxRailcarsCount) DO
    IF NOT EXISTS(SELECT * FROM railcars
      WHERE Train_Number = Train_Number_IN AND Railcar_Number = i) THEN
        INSERT INTO TMPRailcarListCount (Railcar) VALUES (i);
      END IF;
      SET i = i + 1;
  END WHILE;
  SELECT Railcar FROM TMPRailcarListCount;
END$$
DELIMITER;


DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowMaxStopCount;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowMaxStopCount()
  DETERMINISTIC
BEGIN
  DECLARE i int;
  SET i = 1;
  DROP TABLE IF EXISTS TMPStopCount;
  CREATE TEMPORARY TABLE TMPStopCount(
    `Stop_ID` int AUTO_INCREMENT,
    `Stop_Number` int,
    PRIMARY KEY (`Stop_ID`)
  );
  WHILE (i <> 20 + 1) DO
    INSERT INTO TMPStopCount (Stop_Number) VALUES (i);
    SET i = i + 1;
  END WHILE;
  SELECT Stop_ID FROM TMPStopCount;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowRailcarListCount;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowRailcarListCount()
BEGIN
  DECLARE i int;
  SET i = 1;
  DROP TABLE IF EXISTS TMPRailcarListCount;
	CREATE TEMPORARY table TMPRailcarListCount(
		`RailcarCount_ID` int AUTO_INCREMENT,
		`Railcar` int,
		PRIMARY KEY (`RailcarCount_ID`)
	);
  WHILE (i <> 20 + 1) DO
    INSERT INTO TMPRailcarListCount (Railcar) VALUES (i);
    SET i = i + 1;
  END WHILE;
  SELECT RailcarCount_ID FROM TMPRailcarListCount;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowRoutsNames;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowRoutsNames()
BEGIN
  SELECT Rout_Name FROM routes;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd._DISPATCHER_ThrowTrainTypes;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd._DISPATCHER_ThrowTrainTypes()
BEGIN
  SELECT DISTINCT Train_Type FROM train_types;
END$$
DELIMITER;


/*Admin Scripts*/

DROP PROCEDURE IF EXISTS rzd.ADMIN_ThrowUsers;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.ADMIN_ThrowUsers()
BEGIN
  SELECT user, default_role FROM mysql.user
  WHERE (is_role = 'N') AND (default_role <> '') AND (user <> 'RegMaster')
  AND ((default_role = 'Admin') or (default_role = 'user') or (default_role = 'RZD_Dispatcher') or (default_role = 'Blocked'));
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.ADMIN_CreateUser;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.ADMIN_CreateUser(IN UserLogin varchar(30), IN UserPassword varchar(30), IN UserRole varchar(30))
BEGIN
  SET @query1 = CONCAT('
        CREATE USER "', UserLogin, '"@"localhost" IDENTIFIED BY "', UserPassword, '" '
  );
  PREPARE stmt FROM @query1;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;

  SET @query2 = CONCAT('grant "', UserRole, '" to "', UserLogin, '"@"localhost"');
  PREPARE stmt FROM @query2;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
  SET @query3 = CONCAT('set default role "', UserRole, '"for "', UserLogin, '"@"localhost"');
  PREPARE stmt FROM @query3;
  EXECUTE stmt;
  DEALLOCATE PREPARE stmt;
  FLUSH PRIVILEGES;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.ADMIN_ThrowRoles;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.ADMIN_ThrowRoles()
BEGIN
  SELECT DISTINCT default_role FROM mysql.user
  WHERE (default_role = 'Blocked') or (default_role = 'user') or (default_role = 'RZD_Dispatcher') or (default_role = 'Admin');
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.ADMIN_ChangeRole;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.ADMIN_ChangeRole(IN Username varchar(30),  IN Rolename varchar(30))
BEGIN
  SET @query1 = CONCAT('REVOKE ALL PRIVILEGES ON *.* FROM "', Username,'"@"localhost"');
  PREPARE stmt FROM @query1; EXECUTE stmt;  DEALLOCATE PREPARE stmt;
  set @query2 = CONCAT('grant "', Rolename, '" to "', Username,'"@"localhost"');
  Execute immediate @query2;
  PREPARE stmt FROM @query2; EXECUTE stmt;  DEALLOCATE PREPARE stmt;
  SET @query3 = CONCAT('set default role "', Rolename, '" FOR "', Username,'"@"localhost"');
  execute immediate @query3;
    PREPARE stmt FROM @query3; EXECUTE stmt;  DEALLOCATE PREPARE stmt;
  flush privileges;
END$$
DELIMITER;

DROP PROCEDURE IF EXISTS rzd.ADMIN_RefreshPrivileges;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.ADMIN_RefreshPrivileges()
  DETERMINISTIC
BEGIN
  -- Users Access
  GRANT EXECUTE ON PROCEDURE rzd.FindPassengerWithPersonalData TO user;
  GRANT EXECUTE ON PROCEDURE rzd.ThrowAllStations TO user;
  GRANT EXECUTE ON PROCEDURE rzd.Available_Railcar_Types TO user;
  GRANT EXECUTE ON PROCEDURE rzd.TrainInfoTwo TO user;
  GRANT EXECUTE ON PROCEDURE rzd.FindRout TO user;
  GRANT EXECUTE ON PROCEDURE rzd.ThrowTrainNumbersList TO user;
  GRANT EXECUTE ON PROCEDURE rzd.Available_For_Planting_Seats TO user;
  GRANT EXECUTE ON PROCEDURE rzd.newFindTrainList TO user;
  GRANT EXECUTE ON PROCEDURE rzd.throwPassengerInfo TO user;
  GRANT EXECUTE ON FUNCTION rzd.GetDepartureID TO user;
  GRANT EXECUTE ON FUNCTION rzd.GetArrivalID TO user;
  GRANT EXECUTE ON FUNCTION rzd.FindPassenger TO user;
  GRANT EXECUTE ON FUNCTION rzd.PassengerAddToDB TO user;
  GRANT EXECUTE ON FUNCTION rzd.ThrowRoutID TO user;
  GRANT EXECUTE ON PROCEDURE rzd.throwCountAvailableTickets TO user;
  GRANT EXECUTE ON PROCEDURE rzd.CancelTicket TO user;
  GRANT EXECUTE ON PROCEDURE rzd.throwAvailableTicketsWithInfo TO user;
  GRANT EXECUTE ON PROCEDURE rzd.EmployPlaces TO user;
  GRANT EXECUTE ON PROCEDURE rzd.register_createuser TO user;
  GRANT EXECUTE ON PROCEDURE rzd.throwRailcarInfo TO user;
  -- Blocked Users Access
  GRANT EXECUTE ON PROCEDURE rzd.EmptyProcForBlockedUsers TO Blocked;
  -- Dispatcher Access
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_AddTrain TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_DropTrain TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_UpdateTrain TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_ShowTrains TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_ShowRailcars TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_ShowRoutes TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_ShowStops TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_ShowArrivals TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_ShowDepartures TO RZD_Dispatcher;

  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_DropRailcar TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_UpdateRailcar TO RZD_Dispatcher;

  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_UpdateRout TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_DropRout TO RZD_Dispatcher;

  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_UpdateStop TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_DropStop TO RZD_Dispatcher;

  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_UpdateArrival TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_DropArrival TO RZD_Dispatcher;

  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_UpdateDeparture TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_DropDeparture TO RZD_Dispatcher;
  -- TrainAdd
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowRailcarListCount TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowTrainTypes TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowRoutsNames TO RZD_Dispatcher;
  -- RailcarAdd
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowRailcarTypesList TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowTrainList TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowAvailableRailcarList TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_AddRailcar TO RZD_Dispatcher;
  -- RoutAdd
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowMaxStopCount TO RZD_Dispatcher;
  GRANT EXECUTE ON FUNCTION rzd.DISPATCHER_AddRout TO RZD_Dispatcher;
  -- StopAdd
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowStopNumber TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_AddStop TO RZD_Dispatcher;
  -- Arrival or Departure Add
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowAvailableStopsFindByRout TO RZD_Dispatcher;
  GRANT EXECUTE ON PROCEDURE rzd._DISPATCHER_ThrowAvailableTrainsFindByRout TO RZD_Dispatcher;
  -- Arrival Add
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_AddArrival TO RZD_Dispatcher;
  -- Departure Add
  GRANT EXECUTE ON PROCEDURE rzd.DISPATCHER_AddDeparture TO RZD_Dispatcher;
  -- Admin Proc's
  GRANT EXECUTE ON PROCEDURE rzd.ADMIN_ChangeRole TO Admin;
  GRANT EXECUTE ON PROCEDURE rzd.ADMIN_CreateUser TO Admin;
  GRANT EXECUTE ON PROCEDURE rzd.ADMIN_ThrowRoles TO Admin;
  GRANT EXECUTE ON PROCEDURE rzd.ADMIN_ThrowUsers TO Admin;
  -- GRANT EXECUTE ON PROCEDURE rzd.ADMIN_RefreshPrivileges TO Admin;
  FLUSH PRIVILEGES;
END$$
DELIMITER;
