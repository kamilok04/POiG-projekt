/*M!999999\- enable the sandbox mode */ 
-- MariaDB dump 10.19-11.8.2-MariaDB, for Linux (x86_64)
--
-- Host: mysql-3740706b-poig-projekt.j.aivencloud.com    Database: szkola
-- ------------------------------------------------------
-- Server version	8.0.35

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*M!100616 SET @OLD_NOTE_VERBOSITY=@@NOTE_VERBOSITY, NOTE_VERBOSITY=0 */;

--
-- Table structure for table `adres`
--

DROP TABLE IF EXISTS `adres`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `adres` (
  `id` int NOT NULL AUTO_INCREMENT,
  `adres` char(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `adres`
--

LOCK TABLES `adres` WRITE;
/*!40000 ALTER TABLE `adres` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `adres` VALUES
(1,'Zielona 3'),
(2,'Tuska 65'),
(3,'Myszki Miki 54'),
(7,'ul. Pata i Mata 11');
/*!40000 ALTER TABLE `adres` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `dane_kierunku`
--

DROP TABLE IF EXISTS `dane_kierunku`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `dane_kierunku` (
  `id` int NOT NULL AUTO_INCREMENT,
  `nazwa` char(30) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dane_kierunku`
--

LOCK TABLES `dane_kierunku` WRITE;
/*!40000 ALTER TABLE `dane_kierunku` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `dane_kierunku` VALUES
(1,'Matematyka stosowana'),
(2,'Informatyka'),
(3,'Eksperymenty'),
(4,'Muzyka i śpiew'),
(5,'Matematyka'),
(6,'Murarz tynkarz akrobata'),
(7,'Biologia'),
(8,'Browarnictwo'),
(9,'Chemia'),
(10,'Sport');
/*!40000 ALTER TABLE `dane_kierunku` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `dane_przedmiotu`
--

DROP TABLE IF EXISTS `dane_przedmiotu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `dane_przedmiotu` (
  `id` int NOT NULL AUTO_INCREMENT,
  `kod` char(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nazwa` char(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_opisu` int DEFAULT NULL,
  `id_literatury` int DEFAULT NULL,
  `id_warunkow` int DEFAULT NULL,
  `punkty` smallint NOT NULL,
  `wydzial_org` char(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_opisu` (`id_opisu`),
  KEY `id_warunkow` (`id_warunkow`),
  KEY `id_literatury` (`id_literatury`),
  KEY `fk_dane_przedmiotu_1` (`wydzial_org`),
  CONSTRAINT `dane_przedmiotu_ibfk_1` FOREIGN KEY (`id_opisu`) REFERENCES `opis` (`id`),
  CONSTRAINT `dane_przedmiotu_ibfk_2` FOREIGN KEY (`id_warunkow`) REFERENCES `warunki_zaliczenia` (`id`),
  CONSTRAINT `dane_przedmiotu_ibfk_3` FOREIGN KEY (`id_literatury`) REFERENCES `literatura` (`id`),
  CONSTRAINT `fk_dane_przedmiotu_1` FOREIGN KEY (`wydzial_org`) REFERENCES `wydzial` (`nazwa_krotka`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dane_przedmiotu`
--

LOCK TABLES `dane_przedmiotu` WRITE;
/*!40000 ALTER TABLE `dane_przedmiotu` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `dane_przedmiotu` VALUES
(1,'AM101','Analiza Matematyczna',1,1,1,6,'MS'),
(3,'SK203','Sieci Komputerowe',3,3,3,4,'AEI'),
(8,'BIO47','Biologia',13,13,13,5,'BI'),
(15,'TYNK','Tynkarstwo',17,17,19,1,'WF'),
(16,'WMRF','Wymrażanie frakcyjne',18,18,20,2,'CHEM');
/*!40000 ALTER TABLE `dane_przedmiotu` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Temporary table structure for view `dane_studenta`
--

DROP TABLE IF EXISTS `dane_studenta`;
/*!50001 DROP VIEW IF EXISTS `dane_studenta`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8mb4;
/*!50001 CREATE VIEW `dane_studenta` AS SELECT
 1 AS `Nr indeksu`,
  1 AS `Imię`,
  1 AS `Nazwisko`,
  1 AS `Wydział`,
  1 AS `Kierunek`,
  1 AS `Semestr` */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `dane_uzytkownika`
--

DROP TABLE IF EXISTS `dane_uzytkownika`;
/*!50001 DROP VIEW IF EXISTS `dane_uzytkownika`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8mb4;
/*!50001 CREATE VIEW `dane_uzytkownika` AS SELECT
 1 AS `login`,
  1 AS `imie`,
  1 AS `nazwisko`,
  1 AS `data_urodzenia`,
  1 AS `email`,
  1 AS `uprawnienia`,
  1 AS `indeks`,
  1 AS `tytul` */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `dane_wydzialu`
--

DROP TABLE IF EXISTS `dane_wydzialu`;
/*!50001 DROP VIEW IF EXISTS `dane_wydzialu`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8mb4;
/*!50001 CREATE VIEW `dane_wydzialu` AS SELECT
 1 AS `Nazwa`,
  1 AS `Skrót` */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `grupa`
--

DROP TABLE IF EXISTS `grupa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `grupa` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_rocznika` int NOT NULL,
  `numer` char(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `podgrupa` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id_rocznika` (`id_rocznika`),
  KEY `podgrupa` (`podgrupa`),
  CONSTRAINT `grupa_ibfk_1` FOREIGN KEY (`id_rocznika`) REFERENCES `rocznik` (`id`),
  CONSTRAINT `grupa_ibfk_2` FOREIGN KEY (`podgrupa`) REFERENCES `grupa` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grupa`
--

LOCK TABLES `grupa` WRITE;
/*!40000 ALTER TABLE `grupa` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `grupa` VALUES
(1,15,'2/4',NULL),
(12,7,'1/2',NULL),
(15,16,'1',NULL),
(16,17,'1',NULL),
(17,18,'4',NULL);
/*!40000 ALTER TABLE `grupa` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `grupa_przedmiot_prowadzacy`
--

DROP TABLE IF EXISTS `grupa_przedmiot_prowadzacy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `grupa_przedmiot_prowadzacy` (
  `id_grupy` int NOT NULL,
  `id_przedmiotu` int NOT NULL,
  `id_prowadzacego` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id_grupy`,`id_przedmiotu`,`id_prowadzacego`),
  KEY `id_przedmiotu` (`id_przedmiotu`),
  KEY `fk_grupa_przedmiot_prowadzacy_1_idx` (`id_prowadzacego`),
  CONSTRAINT `fk_grupa_przedmiot_prowadzacy_1` FOREIGN KEY (`id_prowadzacego`) REFERENCES `prowadzacy` (`login`),
  CONSTRAINT `grupa_przedmiot_prowadzacy_ibfk_2` FOREIGN KEY (`id_przedmiotu`) REFERENCES `przedmiot` (`id`),
  CONSTRAINT `grupa_przedmiot_prowadzacy_ibfk_3` FOREIGN KEY (`id_grupy`) REFERENCES `grupa` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grupa_przedmiot_prowadzacy`
--

LOCK TABLES `grupa_przedmiot_prowadzacy` WRITE;
/*!40000 ALTER TABLE `grupa_przedmiot_prowadzacy` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `grupa_przedmiot_prowadzacy` VALUES
(1,1,'marcin.chrobok'),
(12,1,'b.mikita'),
(12,2,'a.slowacki');
/*!40000 ALTER TABLE `grupa_przedmiot_prowadzacy` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `grupa_student`
--

DROP TABLE IF EXISTS `grupa_student`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `grupa_student` (
  `indeks` int NOT NULL,
  `id_grupy` int NOT NULL,
  PRIMARY KEY (`indeks`,`id_grupy`),
  KEY `id_grupy` (`id_grupy`),
  CONSTRAINT `grupa_student_ibfk_1` FOREIGN KEY (`indeks`) REFERENCES `student` (`indeks`),
  CONSTRAINT `grupa_student_ibfk_3` FOREIGN KEY (`id_grupy`) REFERENCES `grupa` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grupa_student`
--

LOCK TABLES `grupa_student` WRITE;
/*!40000 ALTER TABLE `grupa_student` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `grupa_student` VALUES
(2133,1),
(2134,1),
(2135,1),
(3004,1),
(3005,1),
(2134,12),
(2133,15),
(2134,15),
(3000,15),
(3001,15),
(3002,15),
(2130,16),
(2131,16),
(2134,16),
(2138,16);
/*!40000 ALTER TABLE `grupa_student` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `grupy_przedmioty`
--

DROP TABLE IF EXISTS `grupy_przedmioty`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `grupy_przedmioty` (
  `id_grupy` int NOT NULL,
  `id_przedmiotu` int NOT NULL,
  PRIMARY KEY (`id_grupy`,`id_przedmiotu`),
  KEY `id_przedmiotu` (`id_przedmiotu`),
  CONSTRAINT `grupy_przedmioty_ibfk_1` FOREIGN KEY (`id_grupy`) REFERENCES `grupa` (`id`),
  CONSTRAINT `grupy_przedmioty_ibfk_2` FOREIGN KEY (`id_przedmiotu`) REFERENCES `przedmiot` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grupy_przedmioty`
--

LOCK TABLES `grupy_przedmioty` WRITE;
/*!40000 ALTER TABLE `grupy_przedmioty` DISABLE KEYS */;
set autocommit=0;
/*!40000 ALTER TABLE `grupy_przedmioty` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `kierunek`
--

DROP TABLE IF EXISTS `kierunek`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `kierunek` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_wydzialu` char(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_danych_kierunku` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_danych_kierunku` (`id_danych_kierunku`),
  KEY `fk_kierunek_1` (`id_wydzialu`),
  CONSTRAINT `fk_kierunek_1` FOREIGN KEY (`id_wydzialu`) REFERENCES `wydzial` (`nazwa_krotka`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `kierunek_ibfk_2` FOREIGN KEY (`id_danych_kierunku`) REFERENCES `dane_kierunku` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `kierunek`
--

LOCK TABLES `kierunek` WRITE;
/*!40000 ALTER TABLE `kierunek` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `kierunek` VALUES
(1,'MS',1),
(2,'MS',2),
(3,'GEO',3),
(4,'MMMM',4),
(10,'MS',2),
(11,'MMMM',4),
(13,'MS',2),
(14,'MS',5),
(15,'WF',6),
(16,'BI',7),
(17,'CHEM',8),
(18,'CHEM',9),
(19,'WF',10);
/*!40000 ALTER TABLE `kierunek` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `literatura`
--

DROP TABLE IF EXISTS `literatura`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `literatura` (
  `id` int NOT NULL AUTO_INCREMENT,
  `literatura` tinytext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `literatura`
--

LOCK TABLES `literatura` WRITE;
/*!40000 ALTER TABLE `literatura` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `literatura` VALUES
(1,'\"Analiza Matematyczna\" autorstwa W. Rudin'),
(3,'\"Computer Networks\", A. Tanenbaum'),
(8,'\"Genetyka\", Peter J. Russella'),
(12,'Geologia, M.Turk'),
(13,'\"Biologia\", M.Turk'),
(17,'\"Nasz nowy dom\" - POLSAT\r\n\"Dom nie do poznania\" - HGTV'),
(18,'\"Świat według Kiepskich\" - POLSAT');
/*!40000 ALTER TABLE `literatura` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `miejsce`
--

DROP TABLE IF EXISTS `miejsce`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `miejsce` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_wydzialu` char(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_adresu` int NOT NULL,
  `numer` char(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  `pojemnosc` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_adresu` (`id_adresu`),
  KEY `fk_miejsce_1` (`id_wydzialu`),
  CONSTRAINT `fk_miejsce_1` FOREIGN KEY (`id_wydzialu`) REFERENCES `wydzial` (`nazwa_krotka`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `miejsce_ibfk_2` FOREIGN KEY (`id_adresu`) REFERENCES `adres` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `miejsce`
--

LOCK TABLES `miejsce` WRITE;
/*!40000 ALTER TABLE `miejsce` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `miejsce` VALUES
(1,'MS',1,'32',6),
(2,'MS',2,'76',47),
(3,'BI',3,'228',31),
(7,'WF',7,'1',147);
/*!40000 ALTER TABLE `miejsce` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `opis`
--

DROP TABLE IF EXISTS `opis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `opis` (
  `id` int NOT NULL AUTO_INCREMENT,
  `opis` tinytext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `opis`
--

LOCK TABLES `opis` WRITE;
/*!40000 ALTER TABLE `opis` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `opis` VALUES
(1,'Wprowadzenie do analizy matematycznej, granice, pochodne, całki.'),
(3,'Podstawy sieci komputerowych, protokoły, topologie.'),
(8,'Podstawy genetyki, dziedziczenie, mutacje'),
(12,'Podstawy geologii, struktura ziemi'),
(13,'Podstawy biologii, w tym anatomii'),
(17,'Obsługa szpachli, rozrabianie tynku z piachem, mieszanie cementu głową'),
(18,'Temperatura krzepnięcia cieczy. Metody sublimacji i resublimacji. Zastosowania w przemyśle browarniczym.'),
(21,'Nauka o bazach danych (MySQL)');
/*!40000 ALTER TABLE `opis` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `prowadzacy`
--

DROP TABLE IF EXISTS `prowadzacy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `prowadzacy` (
  `login` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `tytul` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`login`),
  CONSTRAINT `fk_prowadzacy_1` FOREIGN KEY (`login`) REFERENCES `uzytkownik` (`login`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prowadzacy`
--

LOCK TABLES `prowadzacy` WRITE;
/*!40000 ALTER TABLE `prowadzacy` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `prowadzacy` VALUES
('a.slowacki','lic.'),
('b.mikita','dr inż. n. med.'),
('j.bartosik','inż.'),
('marcin.chrobok','mgr'),
('p.piwowski','lic.'),
('p.student','inż.'),
('s.pernal','mgr'),
('t.niegot','nie'),
('t.starszy','prof. dr hab.');
/*!40000 ALTER TABLE `prowadzacy` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `prowadzacy_przedmioty`
--

DROP TABLE IF EXISTS `prowadzacy_przedmioty`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `prowadzacy_przedmioty` (
  `id_prowadzacego` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_przedmiotu` int NOT NULL,
  PRIMARY KEY (`id_prowadzacego`,`id_przedmiotu`),
  KEY `id_prowadzacego` (`id_prowadzacego`),
  KEY `id_przedmiotu` (`id_przedmiotu`),
  CONSTRAINT `fk_prowadzacy_przedmioty_1` FOREIGN KEY (`id_prowadzacego`) REFERENCES `prowadzacy` (`login`) ON UPDATE CASCADE,
  CONSTRAINT `prowadzacy_przedmioty_ibfk_2` FOREIGN KEY (`id_przedmiotu`) REFERENCES `przedmiot` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prowadzacy_przedmioty`
--

LOCK TABLES `prowadzacy_przedmioty` WRITE;
/*!40000 ALTER TABLE `prowadzacy_przedmioty` DISABLE KEYS */;
set autocommit=0;
/*!40000 ALTER TABLE `prowadzacy_przedmioty` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `przedmiot`
--

DROP TABLE IF EXISTS `przedmiot`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `przedmiot` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_danych` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_danych` (`id_danych`),
  CONSTRAINT `przedmiot_ibfk_1` FOREIGN KEY (`id_danych`) REFERENCES `dane_przedmiotu` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `przedmiot`
--

LOCK TABLES `przedmiot` WRITE;
/*!40000 ALTER TABLE `przedmiot` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `przedmiot` VALUES
(1,1),
(2,3),
(7,8),
(14,15),
(15,16);
/*!40000 ALTER TABLE `przedmiot` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `rocznik`
--

DROP TABLE IF EXISTS `rocznik`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `rocznik` (
  `id` int NOT NULL AUTO_INCREMENT,
  `id_kierunku` int NOT NULL,
  `semestr` smallint NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_kierunku` (`id_kierunku`),
  CONSTRAINT `rocznik_ibfk_1` FOREIGN KEY (`id_kierunku`) REFERENCES `kierunek` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rocznik`
--

LOCK TABLES `rocznik` WRITE;
/*!40000 ALTER TABLE `rocznik` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `rocznik` VALUES
(1,1,4),
(2,3,4),
(3,3,6),
(4,3,2),
(5,3,4),
(6,3,6),
(7,2,1),
(8,4,2),
(9,4,1),
(10,2,1),
(11,2,7),
(12,1,5),
(13,3,3),
(14,2,4),
(15,14,4),
(16,15,1),
(17,17,7),
(18,18,3);
/*!40000 ALTER TABLE `rocznik` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `sesje`
--

DROP TABLE IF EXISTS `sesje`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `sesje` (
  `login` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `token` char(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `data_waznosci` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `uprawnienia` int NOT NULL,
  PRIMARY KEY (`login`),
  UNIQUE KEY `login` (`login`),
  CONSTRAINT `sesje_ibfk_1` FOREIGN KEY (`login`) REFERENCES `uzytkownik` (`login`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sesje`
--

LOCK TABLES `sesje` WRITE;
/*!40000 ALTER TABLE `sesje` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `sesje` VALUES
('a.slowacki','WxHRYmSmQIXxFlMW4NZUlZB1kgkRFYfu0UlctH8ueQ9Sah1rffhyzJJGf5NS4AOv1dGpWtknTLey1TbF9hyEgw==','2025-07-09 14:13:46',53),
('marcin.chrobok','7r+LfCFTPXxCDB8CzySFjPxPNpTRUtjTn09ylOmgEGUmoOTIFq/ZvKzNfhlWQIc7vTaHQtFDaodrBTUqsfuGkQ==','2025-07-09 13:28:03',31),
('p.piwowski','6ijS6nxeBzFNPnYQhQG83a8OUOxgGnpUg8smdEcXkVsf7TstJc0uF8wmKFCY5Ves4Sf3r0v3kkEpOPiUloltYw==','2025-07-09 14:04:47',-1);
/*!40000 ALTER TABLE `sesje` ENABLE KEYS */;
UNLOCK TABLES;
commit;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`avnadmin`@`%`*/ /*!50003 TRIGGER `domyslna_sesja` BEFORE INSERT ON `sesje` FOR EACH ROW SET new.data_waznosci = DATE_ADD(CURRENT_TIMESTAMP, INTERVAL 1 HOUR) */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Table structure for table `student`
--

DROP TABLE IF EXISTS `student`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `student` (
  `login` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `indeks` int NOT NULL,
  PRIMARY KEY (`login`),
  UNIQUE KEY `indeks` (`indeks`),
  CONSTRAINT `fk_student_1` FOREIGN KEY (`login`) REFERENCES `uzytkownik` (`login`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `student`
--

LOCK TABLES `student` WRITE;
/*!40000 ALTER TABLE `student` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `student` VALUES
('p.piwowski',123),
('b.mikita',2130),
('s.pernal',2131),
('m.zaczyk',2132),
('k.prykhodko',2133),
('a.luba',2134),
('i.gaudyn',2135),
('f.waligora',2136),
('m.szmuc',2137),
('p.student',2138),
('janusz.kowalski',3000),
('tadeusz.fujara',3001),
('t.starszy',3002),
('i.najman',3003),
('jan.jan',3004),
('marek.jankowski',3005);
/*!40000 ALTER TABLE `student` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `uzytkownik`
--

DROP TABLE IF EXISTS `uzytkownik`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `uzytkownik` (
  `login` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `imie` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nazwisko` char(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `data_urodzenia` date DEFAULT NULL,
  `email` char(40) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `salt` char(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `haslo` char(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `uprawnienia` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`login`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `uzytkownik`
--

LOCK TABLES `uzytkownik` WRITE;
/*!40000 ALTER TABLE `uzytkownik` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `uzytkownik` VALUES
('a.adamowski','Adam','Adamowski','2025-06-19','a.adamowski@email.com','728DAFCAA95C189D3106','DD9ECC336BE16A5F2ADE134EFAC04E7B101B00BF2F386145B5654C77E284912E',3),
('a.luba','Alojzy','Luba','2005-04-02','a.luba@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',15),
('a.nowak','Adam','Nowak','1999-02-04','a.nowak@email.com','5F500456E10281734DC3','D5E4533065E88A6AC0F2DF18F31D639AC07C1BE60807ABA815E48101CD6CF113',1996),
('a.slowacki','Adam','Słowacki','1798-12-24','czterdziesciicztery@yahoo.com','5C04CCC4146A768755D3','64D98A5362C4D04753DCCF582E3795E69EE6585013D5E2BE21B89DD35478D160',53),
('a.wasilewski','Aleksander','Wasilewski','2005-04-02','a.wasilewski@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',15),
('b.mikita','Bernadeta','Mikita','2005-04-02','b.mikita@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',7),
('f.waligora','Franciszka','Waligóra','2005-04-02','f.waligora@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',1),
('i.gaudyn','Irena','Gaudyn','2005-04-02','i.gaudyn@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',1),
('i.najman','Ignacy','Najman','1999-05-25','i.najman@email.com','DB274B25D9EF6EB34F0B','7B324A01F5195D6409F642A6B764ED6D668146430BDA391134DBD084DEDC1132',0),
('imie.nazwisko','imie','nazwisko','2005-05-05','email@email.com','44768345AC223559144E','9B83CEAC35B4160CB21857DDD075F4837EFD7F69D3846750DEF940B25EB7E865',8),
('j.bartosik','Janina','Bartosik','2005-04-02','j.bartosik@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',1055),
('j.nowak','Jan','Nowak','1999-02-05','j.nowak@email.com','89A007BC75CBD55969EA','D1078955CC3F3CC32B4033374011030DF8949D69C50168263CBB13BE8DC065BB',-1),
('jan.jan','Jan','Kowalski','2004-06-09','jan.kowalski@jan.pl','CFC42D796F56B8A9F87E','B1EE82F869E31E3251D3B1171D77A7EBE23882C152AD5F1BD7F4ECE783061E01',3),
('janusz.kowalski','Janusz','Kowalski','2002-07-12','janusz.kowalski@email.com','3B5421178C3F02DFC5C6','511C5F569EFA7CA6C368937F4F3182F731497A2BA90A5E8106118E4B4FBCAD33',8),
('k.prykhodko','Karol','Prykhodko','2005-04-02','k.prykhodko@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',7),
('m.szmuc','Maksym','Szmuc','2005-04-02','m.szmuc@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',1),
('m.zaczyk','Malwina','Zaczyk','2005-04-02','m.zaczyk@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',1),
('marcin.chrobok','Marcin','Chrobok','1990-03-08','marcin.chrobok@email.com','F27CBE90FD3C80CB4AC9','0922AEFFF82FB8A20CEFC8661ED4846C01CA997B6FCD1DB05110B7441C248CF0',31),
('marek.jankowski','Marek','Jankowski','1990-03-16','marek.jankowski@email.com','75AA6DD6D3444CA735F8','329C4A5ECCB37DFCBD91290C50E99DB51A052623A1709CE6B87F5A5BB262A850',3),
('p.piwowski','Piotr','Piwowski','2005-04-02','p.piwowski@email.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',-1),
('p.student','Piotr','Student','2005-05-05','p.student@email.com','D28A71A3E0DE2E474FDE','1D70A2C5A7D1458850556922BC76EA52A12C3FC4E04528100FEEE5AC5FA7D510',0),
('robert.kubica','Robert','Kubica','2012-11-11','robert.kubica@student.polsl.pl','093D932E5360A1606E92','7D467EB8DC63A6FDEA19CE99342517606FA29CDEFE45B09846C01365A3D4F602',3),
('s.pernal','Stefan','Pernal','2005-04-02','s.pernal@gmail.com','DpbBqDs0p&zOXA/rJvRY','B6E1AD3F2B536818337F849B5C8BF9B71B791937AF9DFA97B1CBC1E450FBE31A',1),
('t.niegot','Tadeusz','Niegot','1999-11-18','tadeusz.niegot@gmail.com','7FB34576ECB7CA2AEA36','E6E2E5D2B76D56D79532D3B4A838EAFABC9C2FBAD18D280E971565BF439570AE',0),
('t.starszy','Tadeusz','Starszy','2025-06-05','tadeusz.starszy@gmail.com','3DA9E1162855D40A9D63','51B0A5199A36147B0366AF07897A60404D472EB71EBC1624F9C6FAD370553AD7',0),
('tadeusz.fujara','Tadeusz','Fujara','2003-02-13','tadeusz.fujara@gmail.com','734CEEA5FB74BB2BA8C2','21CAB05278D1EE652E7CFA133880389BC5C50B89AE26C3A3F41B0B845273259E',0);
/*!40000 ALTER TABLE `uzytkownik` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `warunki_zaliczenia`
--

DROP TABLE IF EXISTS `warunki_zaliczenia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `warunki_zaliczenia` (
  `id` int NOT NULL AUTO_INCREMENT,
  `warunki_zaliczenia` tinytext COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `warunki_zaliczenia`
--

LOCK TABLES `warunki_zaliczenia` WRITE;
/*!40000 ALTER TABLE `warunki_zaliczenia` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `warunki_zaliczenia` VALUES
(1,'Egzamin pisemny'),
(3,'Kolokwium zaliczeniowe'),
(8,'Kolokwium zaliczeniowe'),
(12,'Egzamin'),
(13,'Ćwiczenia i kolokwia, skuteczna sekcja zwłok'),
(17,'Egzamin ustny'),
(18,''),
(19,'Kolokwium i laboratoria'),
(20,'Egzamin praktyczny ');
/*!40000 ALTER TABLE `warunki_zaliczenia` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `wydzial`
--

DROP TABLE IF EXISTS `wydzial`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `wydzial` (
  `nazwa` char(40) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nazwa_krotka` char(5) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`nazwa_krotka`),
  UNIQUE KEY `nazwa_UNIQUE` (`nazwa`),
  UNIQUE KEY `nazwa_krotka_UNIQUE` (`nazwa_krotka`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wydzial`
--

LOCK TABLES `wydzial` WRITE;
/*!40000 ALTER TABLE `wydzial` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `wydzial` VALUES
('Automatyki, Elektroniki i Informatyki','AEI'),
('Chemii','CHEM'),
('Instytut Fizyki','IF'),
('Matematyki i Statystyki','MS'),
('Mechaniki i Technologii','MT'),
('Wychowania fizycznego','WF'),
('Wydział Biologiczny','BI'),
('Wydział Geograficzny','GEO'),
('Wydział Muzyki i Śpiewu','MMMM');
/*!40000 ALTER TABLE `wydzial` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Table structure for table `zajecie`
--

DROP TABLE IF EXISTS `zajecie`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8mb4 */;
CREATE TABLE `zajecie` (
  `id` int NOT NULL AUTO_INCREMENT,
  `dzien_tygodnia` enum('poniedziałek','wtorek','środa','czwartek','piątek','sobota','niedziela') COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `godzina_start` time NOT NULL,
  `godzina_stop` time NOT NULL,
  `id_przedmiotu` int NOT NULL,
  `id_grupy` int NOT NULL,
  `id_miejsca` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id_przedmiotu` (`id_przedmiotu`),
  KEY `id_miejsca` (`id_miejsca`),
  KEY `fk_zajecie_1_idx` (`id_grupy`),
  CONSTRAINT `fk_zajecie_1` FOREIGN KEY (`id_grupy`) REFERENCES `grupa` (`id`) ON DELETE CASCADE,
  CONSTRAINT `zajecie_ibfk_1` FOREIGN KEY (`id_przedmiotu`) REFERENCES `przedmiot` (`id`),
  CONSTRAINT `zajecie_ibfk_2` FOREIGN KEY (`id_miejsca`) REFERENCES `miejsce` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `zajecie`
--

LOCK TABLES `zajecie` WRITE;
/*!40000 ALTER TABLE `zajecie` DISABLE KEYS */;
set autocommit=0;
INSERT INTO `zajecie` VALUES
(1,'czwartek','12:00:00','13:30:00',2,1,2),
(2,'czwartek','12:00:00','13:30:00',7,1,3),
(3,'czwartek','22:00:00','23:30:00',7,1,1),
(4,'niedziela','22:00:00','23:30:00',1,1,2),
(5,'poniedziałek','22:00:00','23:30:00',1,1,1),
(7,'poniedziałek','08:30:00','10:00:00',1,1,2),
(8,'piątek','13:45:00','15:15:00',2,1,2),
(9,'wtorek','08:00:00','09:03:00',2,1,1),
(11,'wtorek','20:03:00','21:37:00',14,15,7),
(12,'wtorek','12:00:00','13:30:00',15,16,7),
(13,'poniedziałek','12:00:00','13:30:00',7,15,1);
/*!40000 ALTER TABLE `zajecie` ENABLE KEYS */;
UNLOCK TABLES;
commit;

--
-- Dumping events for database 'szkola'
--
/*!50106 SET @save_time_zone= @@TIME_ZONE */ ;
/*!50106 DROP EVENT IF EXISTS `usun_stare_sesje` */;
DELIMITER ;;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;;
/*!50003 SET character_set_client  = utf8mb4 */ ;;
/*!50003 SET character_set_results = utf8mb4 */ ;;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;;
/*!50003 SET @saved_time_zone      = @@time_zone */ ;;
/*!50003 SET time_zone             = 'SYSTEM' */ ;;
/*!50106 CREATE*/ /*!50117 DEFINER=`avnadmin`@`%`*/ /*!50106 EVENT `usun_stare_sesje` ON SCHEDULE EVERY 1 DAY STARTS '2025-06-23 21:38:46' ON COMPLETION PRESERVE ENABLE COMMENT 'Usuń wygasłe i nieużywane sesje.' DO DELETE FROM sesje
	WHERE data_waznosci < DATE_ADD(CURRENT_TIMESTAMP, INTERVAL -1 HOUR) */ ;;
/*!50003 SET time_zone             = @saved_time_zone */ ;;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;;
/*!50003 SET character_set_client  = @saved_cs_client */ ;;
/*!50003 SET character_set_results = @saved_cs_results */ ;;
/*!50003 SET collation_connection  = @saved_col_connection */ ;;
DELIMITER ;
/*!50106 SET TIME_ZONE= @save_time_zone */ ;

--
-- Dumping routines for database 'szkola'
--
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `AddDegree` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "AddDegree"(
    IN p_nazwa CHAR(30),
    IN p_nazwa_krotka CHAR(5)
)
BEGIN
    DECLARE v_id_danych INT;

    START TRANSACTION;

    INSERT INTO dane_kierunku (nazwa) VALUES (p_nazwa);
    SELECT LAST_INSERT_ID() INTO v_id_danych;

    INSERT INTO kierunek (id_danych_kierunku, id_wydzialu) VALUES (v_id_danych, p_nazwa_krotka);

    COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `AddGroup` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "AddGroup"(
    IN p_groupnumber CHAR(5),
    IN p_currentFaculty CHAR(5),
    IN p_currentDegree CHAR(30),
    IN p_currentSemester SMALLINT
)
BEGIN
    DECLARE v_dane_kierunku_id INT;
    DECLARE v_kierunek_id INT;
    DECLARE v_rocznik_id INT;

    IF NOT EXISTS (SELECT 1 FROM wydzial WHERE nazwa_krotka = p_currentFaculty) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Faculty does not exist.';
    END IF;

    SELECT id INTO v_dane_kierunku_id
    FROM dane_kierunku
    WHERE nazwa = p_currentDegree
    LIMIT 1;

    IF v_dane_kierunku_id IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Degree does not exist.';
    END IF;

    SELECT id INTO v_kierunek_id
    FROM kierunek
    WHERE id_wydzialu = p_currentFaculty
    AND id_danych_kierunku = v_dane_kierunku_id
    LIMIT 1;

    IF v_kierunek_id IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Kierunek does not exist for the given faculty and degree.';
    END IF;

    SELECT id INTO v_rocznik_id
    FROM rocznik
    WHERE id_kierunku = v_kierunek_id
    AND semestr = p_currentSemester
    LIMIT 1;

    IF v_rocznik_id IS NULL THEN
        INSERT INTO rocznik (id_kierunku, semestr)
        VALUES (v_kierunek_id, p_currentSemester);
        SET v_rocznik_id = LAST_INSERT_ID();
    END IF;

    INSERT INTO grupa (id_rocznika, numer)
    VALUES (v_rocznik_id, p_groupnumber);
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `AddSubject` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "AddSubject"(
    IN p_description TINYTEXT,
    IN p_literature TINYTEXT,
    IN p_passingCriteria TINYTEXT,
    IN p_code CHAR(10),
    IN p_name CHAR(50),
    IN p_points SMALLINT,
    IN p_currentFaculty CHAR(5)
)
BEGIN
    DECLARE v_id_opisu INT;
    DECLARE v_id_literatury INT;
    DECLARE v_id_warunkow INT;
    DECLARE v_id_danych INT;

    START TRANSACTION;
    
    IF p_points IS NULL OR p_points < 1 OR p_points > 6 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Points must be between 1 and 6';
    END IF;

    INSERT INTO opis (opis) VALUES (p_description);
    SELECT LAST_INSERT_ID() INTO v_id_opisu;

    INSERT INTO literatura (literatura) VALUES (p_literature);
    SELECT LAST_INSERT_ID() INTO v_id_literatury;

    INSERT INTO warunki_zaliczenia (warunki_zaliczenia) VALUES (p_passingCriteria);
    SELECT LAST_INSERT_ID() INTO v_id_warunkow;

    INSERT INTO dane_przedmiotu (kod, nazwa, id_opisu, id_literatury, id_warunkow, punkty, wydzial_org)
    VALUES (p_code, p_name, v_id_opisu, v_id_literatury, v_id_warunkow, p_points, p_currentFaculty);
    SELECT LAST_INSERT_ID() INTO v_id_danych;

    INSERT INTO przedmiot (id_danych) VALUES (v_id_danych);

    COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteDegree` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "DeleteDegree"(
    IN p_nazwa CHAR(30),
    IN p_nazwa_krotka CHAR(5)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'An error occurred during deletion';
    END;

    START TRANSACTION;

    DELETE FROM kierunek
    WHERE id_wydzialu = p_nazwa_krotka
    AND id_danych_kierunku IN (SELECT id FROM dane_kierunku WHERE nazwa = p_nazwa);

    COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteGroup` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "DeleteGroup"(
    IN p_group_id INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM grupa WHERE id = p_group_id) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Group does not exist.';
    END IF;

    DELETE FROM grupa_student 
    WHERE id_grupy = p_group_id;
    
    DELETE FROM grupy_przedmioty
    WHERE id_grupy = p_group_id;
    
    DELETE FROM grupa_przedmiot_prowadzacy
    WHERE id_grupy = p_group_id;

    DELETE FROM grupa 
    WHERE id = p_group_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `DeleteSubject` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "DeleteSubject"(
    IN p_id_dane_przedmiotu INT
)
BEGIN
    DECLARE v_id_opisu INT;
    DECLARE v_id_literatury INT;
    DECLARE v_id_warunkow INT;

    START TRANSACTION;

    SELECT id_opisu, id_literatury, id_warunkow 
    INTO v_id_opisu, v_id_literatury, v_id_warunkow
    FROM dane_przedmiotu
    WHERE id = p_id_dane_przedmiotu;

    IF v_id_opisu IS NOT NULL THEN
        DELETE z FROM zajecie z
        JOIN przedmiot p ON z.id_przedmiotu = p.id
        WHERE p.id_danych = p_id_dane_przedmiotu;

        DELETE gpp FROM grupa_przedmiot_prowadzacy gpp
        JOIN przedmiot p ON gpp.id_przedmiotu = p.id
        WHERE p.id_danych = p_id_dane_przedmiotu;

        DELETE gp FROM grupy_przedmioty gp
        JOIN przedmiot p ON gp.id_przedmiotu = p.id
        WHERE p.id_danych = p_id_dane_przedmiotu;

        DELETE FROM przedmiot
        WHERE id_danych = p_id_dane_przedmiotu;

        DELETE FROM dane_przedmiotu
        WHERE id = p_id_dane_przedmiotu;

        DELETE FROM opis
        WHERE id = v_id_opisu;

        DELETE FROM literatura
        WHERE id = v_id_literatury;

        DELETE FROM warunki_zaliczenia
        WHERE id = v_id_warunkow;
    END IF;

    COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GetGroupId` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "GetGroupId"(
    IN p_groupnumber CHAR(5),
    IN p_currentFaculty CHAR(5),
    IN p_currentDegree CHAR(30),
    IN p_currentSemester SMALLINT
)
BEGIN
    SELECT g.id
    FROM grupa g
    JOIN rocznik r ON g.id_rocznika = r.id
    JOIN kierunek k ON r.id_kierunku = k.id
    JOIN wydzial w ON k.id_wydzialu = w.nazwa_krotka
    JOIN dane_kierunku dk ON k.id_danych_kierunku = dk.id
    WHERE g.numer = p_groupnumber
    AND r.semestr = p_currentSemester
    AND w.nazwa_krotka = p_currentFaculty
    AND dk.nazwa = p_currentDegree;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateGroup` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "UpdateGroup"(
    IN p_group_id INT,
    IN p_groupnumber CHAR(5),
    IN p_currentFaculty CHAR(5),
    IN p_currentDegree CHAR(30),
    IN p_currentSemester SMALLINT
)
BEGIN
    DECLARE v_dane_kierunku_id INT;
    DECLARE v_kierunek_id INT;
    DECLARE v_rocznik_id INT;

    IF NOT EXISTS (SELECT 1 FROM grupa WHERE id = p_group_id) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Group does not exist.';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM wydzial WHERE nazwa_krotka = p_currentFaculty) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Faculty does not exist.';
    END IF;

    SELECT id INTO v_dane_kierunku_id
    FROM dane_kierunku
    WHERE nazwa = p_currentDegree
    LIMIT 1;

    IF v_dane_kierunku_id IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Degree does not exist.';
    END IF;

    SELECT id INTO v_kierunek_id
    FROM kierunek
    WHERE id_wydzialu = p_currentFaculty
    AND id_danych_kierunku = v_dane_kierunku_id
    LIMIT 1;

    IF v_kierunek_id IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Kierunek does not exist for the given faculty and degree.';
    END IF;

    SELECT id INTO v_rocznik_id
    FROM rocznik
    WHERE id_kierunku = v_kierunek_id
    AND semestr = p_currentSemester
    LIMIT 1;

    IF v_rocznik_id IS NULL THEN
        INSERT INTO rocznik (id_kierunku, semestr)
        VALUES (v_kierunek_id, p_currentSemester);
        SET v_rocznik_id = LAST_INSERT_ID();
    END IF;

    UPDATE grupa
    SET id_rocznika = v_rocznik_id,
        numer = p_groupnumber
    WHERE id = p_group_id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'REAL_AS_FLOAT,PIPES_AS_CONCAT,ANSI_QUOTES,IGNORE_SPACE,ONLY_FULL_GROUP_BY,ANSI,STRICT_ALL_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
/*!50003 DROP PROCEDURE IF EXISTS `UpdateSubject` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
DELIMITER ;;
CREATE DEFINER="avnadmin"@"%" PROCEDURE "UpdateSubject"(
    IN p_dane_przedmiotu_id INT,
    IN p_kod CHAR(10),
    IN p_nazwa CHAR(30),
    IN p_opis TINYTEXT,
    IN p_literatura TINYTEXT,
    IN p_warunki TINYTEXT,
    IN p_punkty SMALLINT,
    IN p_wydzial_nazwa_krotka CHAR(30)
)
BEGIN
    DECLARE v_id_opis INT;
    DECLARE v_id_literatura INT;
    DECLARE v_id_warunki INT;
    DECLARE v_wydzial_exists INT;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_wydzial_exists 
    FROM wydzial 
    WHERE nazwa_krotka = p_wydzial_nazwa_krotka;
    IF v_wydzial_exists = 0 THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Wydzial with provided nazwa_krotka not found';
    END IF;

    SELECT id INTO v_id_opis 
    FROM opis 
    WHERE opis = p_opis 
    LIMIT 1;
    IF v_id_opis IS NULL THEN
        INSERT INTO opis (opis) VALUES (p_opis);
        SET v_id_opis = LAST_INSERT_ID();
    END IF;

    SELECT id INTO v_id_literatura 
    FROM literatura 
    WHERE literatura = p_literatura 
    LIMIT 1;
    IF v_id_literatura IS NULL THEN
        INSERT INTO literatura (literatura) VALUES (p_literatura);
        SET v_id_literatura = LAST_INSERT_ID();
    END IF;

    SELECT id INTO v_id_warunki 
    FROM warunki_zaliczenia 
    WHERE warunki_zaliczenia = p_warunki 
    LIMIT 1;
    IF v_id_warunki IS NULL THEN
        INSERT INTO warunki_zaliczenia (warunki_zaliczenia) VALUES (p_warunki);
        SET v_id_warunki = LAST_INSERT_ID();
    END IF;

    UPDATE dane_przedmiotu
    SET kod = p_kod,
        nazwa = p_nazwa,
        id_opisu = v_id_opis,
        id_literatury = v_id_literatura,
        id_warunkow = v_id_warunki,
        punkty = p_punkty,
        wydzial_org = p_wydzial_nazwa_krotka
    WHERE id = p_dane_przedmiotu_id;

    IF ROW_COUNT() = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Dane_przedmiotu with provided id not found';
    ELSE
        COMMIT;
    END IF;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `dane_studenta`
--

/*!50001 DROP VIEW IF EXISTS `dane_studenta`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`avnadmin`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `dane_studenta` AS select `s`.`indeks` AS `Nr indeksu`,`u`.`imie` AS `Imię`,`u`.`nazwisko` AS `Nazwisko`,`w`.`nazwa_krotka` AS `Wydział`,`dk`.`nazwa` AS `Kierunek`,`r`.`semestr` AS `Semestr` from (((((((`student` `s` left join `uzytkownik` `u` on((`u`.`login` = `s`.`login`))) left join `grupa_student` `gs` on((`gs`.`indeks` = `s`.`indeks`))) left join `grupa` `g` on((`g`.`id` = `gs`.`id_grupy`))) left join `rocznik` `r` on((`g`.`id_rocznika` = `r`.`id`))) left join `kierunek` `k` on((`r`.`id_kierunku` = `k`.`id`))) left join `wydzial` `w` on((`w`.`nazwa_krotka` = `k`.`id_wydzialu`))) left join `dane_kierunku` `dk` on((`dk`.`id` = `k`.`id_danych_kierunku`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `dane_uzytkownika`
--

/*!50001 DROP VIEW IF EXISTS `dane_uzytkownika`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`avnadmin`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `dane_uzytkownika` AS select `u`.`login` AS `login`,`u`.`imie` AS `imie`,`u`.`nazwisko` AS `nazwisko`,`u`.`data_urodzenia` AS `data_urodzenia`,`u`.`email` AS `email`,`u`.`uprawnienia` AS `uprawnienia`,`student`.`indeks` AS `indeks`,`prowadzacy`.`tytul` AS `tytul` from ((`uzytkownik` `u` left join `student` on((`student`.`login` = `u`.`login`))) left join `prowadzacy` on((`prowadzacy`.`login` = `u`.`login`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `dane_wydzialu`
--

/*!50001 DROP VIEW IF EXISTS `dane_wydzialu`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`avnadmin`@`%` SQL SECURITY DEFINER */
/*!50001 VIEW `dane_wydzialu` AS select `wydzial`.`nazwa` AS `Nazwa`,`wydzial`.`nazwa_krotka` AS `Skrót` from `wydzial` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*M!100616 SET NOTE_VERBOSITY=@OLD_NOTE_VERBOSITY */;

-- Dump completed on 2025-07-09 16:52:27
