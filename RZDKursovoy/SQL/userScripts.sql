/*MySQL scripts for DB*/
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
  GRANT EXECUTE ON FUNCTION rzd.ThrowArrivalStopID TO user;
  GRANT EXECUTE ON FUNCTION rzd.ThrowDepartureStopID TO user;

  -- Blocked Users Access
  GRANT EXECUTE ON PROCEDURE rzd.EmptyProcForBlockedUsers TO Blocked;
  FLUSH PRIVILEGES;
END$$
DELIMITER;

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

  SELECT Arrival_railwaystationname INTO RailwayStation
  FROM arrivals
  WHERE Arrival_ID = Arrival_ID_IN;

  IF (RailwayStation is NOT NULL) THEN
    SET FirstStation = CONCAT(FirstStation, ', ', RailwayStation);
  END IF;

  SELECT Departure_railwaystationname INTO RailwayStation
  FROM departures
  WHERE Departure_ID = DepartureID_IN;

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

DROP PROCEDURE IF EXISTS rzd.newFindTrainList;
DELIMITER $$
CREATE DEFINER = 'root'@'localhost'
PROCEDURE rzd.newFindTrainList(
	`Rout_ID_IN` int,
	`Arrival_Stop_Name` varchar(30),
	`Arrival_Date_IN` varchar(30)
)
BEGIN
	DECLARE TrainNumber int;
	DECLARE CurrentStopID int;
  DECLARE CheckDate Date;
  DECLARE trueDate Date;
  SET CheckDate = now();
  SET trueDate = STR_TO_DATE(Arrival_Date_IN, '%d.%m.%Y');
  IF (trueDate > CheckDate) THEN
    SELECT Stop_ID INTO CurrentStopID FROM stops
    WHERE (Rout_ID = Rout_ID_IN) AND (Stop_Name = Arrival_Stop_Name);
    SELECT Train_Number FROM arrivals
    WHERE (Arrival_Date = trueDate) AND (Stop_ID = CurrentStopID) AND (Rout_ID = Rout_ID_IN);
  ELSE
    SELECT -1;
  END IF;
END$$
DELIMITER;

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

    SELECT Arrival_railwaystationName INTO _Arrival_Railway_Station FROM arrivals
    WHERE Arrival_ID = _TMPArrivalID;

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

    SELECT Departure_railwaystationName INTO _Departure_Railway_Station FROM departures
    WHERE Departure_ID = _TMPDepartureID;

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
		signal sqlstate '45000' SET message_text = 'Throw Error #1';
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
