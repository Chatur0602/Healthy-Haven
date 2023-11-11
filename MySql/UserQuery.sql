Create database HHDB;
use HHDB;

CREATE TABLE Users (
  id INT NOT NULL AUTO_INCREMENT,
  firstname VARCHAR(255) NOT NULL,
  lastname VARCHAR(255) NOT NULL,
  email VARCHAR(255) NOT NULL UNIQUE,
  password VARCHAR(255) NOT NULL,
  Gender char NOT NULL,
  Age int NOT NULL,
  role char NOT NULL,
  PRIMARY KEY (id)
);

ALTER TABLE AspNetUsers
DROP Column firstname,
DROP Column lastname,
DROP Column Gender,
DROP Column Age;

ALTER TABLE AspNetUsers
DROP COLUMN firstname;

drop table Users;

Insert Into Users(FirstName, LastName, Email, Password, Gender, Age, Role) values('Nikhil', 'Chaturvedi','Nikhil.Chaturvedi@outlook.my', 'hello1234', 'M', 21, 'M');
Insert Into Users(FirstName, LastName, Email, Password, Gender, Age, Role) values('Hamidreza', 'Malek','reza@gmail.com', 'reza1234', 'M', 23, 'U');
Insert Into Users(FirstName, LastName, Email, Gender, Age, Address) values('Wee', 'Shi Min','shimin@hotmail.com', 'F', 22, 'Somewhere in WP');
Insert Into Users(FirstName, LastName, Email, Gender, Age, Address) values('Assran', 'Mydeeen','assran@yahoo.com', 'M', 22, 'Sri Impian');

DELETE FROM AspNetUsers WHERE id='00115c35-4afd-4fb0-b456-6b7956328c1a';
DELETE FROM AspNetUsers WHERE id='5866cb8e-893e-4cdc-baa7-ad24cbd20b0a';
DELETE FROM AspNetUsers WHERE id='e6d1ca72-22dc-46ed-96b8-21b798da2fda';

SHOW KEYS FROM Users WHERE Key_name = 'PRIMARY';

describe AspNetUsers;

select * from AspNetRoles;

select * from AspNetUsers;

select * from __EFMigrationsHistory;

DROP Table AspNetUsers;

SELECT table_name
FROM information_schema.tables
WHERE table_type='BASE TABLE'
      AND table_schema = 'HHDB'