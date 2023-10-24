create database HHDB;
use HHDB;

CREATE TABLE Users (
  id INT NOT NULL AUTO_INCREMENT,
  firstname VARCHAR(255) NOT NULL,
  lastname VARCHAR(255) NOT NULL,
  email VARCHAR(255) NOT NULL UNIQUE,
  password VARCHAR(255) NOT NULL,
  Gender char NOT NULL,
  Age int NOT NULL,
  role VARCHAR(255) NOT NULL,
  PRIMARY KEY (id)
);

drop table Users;

Insert Into Users(FirstName, LastName, Email, Gender, Age, Address) values('Nikhil', 'Chaturvedi','Nikhil.Chaturvedi@outlook.my', 'M', 21, 'Robson Condominium');
Insert Into Users(FirstName, LastName, Email, Gender, Age, Address) values('Hamidreza', 'Malek','reza@gmail.com', 'M', 23, 'La Casa Grande');
Insert Into Users(FirstName, LastName, Email, Gender, Age, Address) values('Wee', 'Shi Min','shimin@hotmail.com', 'F', 22, 'Somewhere in WP');
Insert Into Users(FirstName, LastName, Email, Gender, Age, Address) values('Assran', 'Mydeeen','assran@yahoo.com', 'M', 22, 'Sri Impian');

DELETE FROM Users WHERE UserId=7;

SHOW KEYS FROM Users WHERE Key_name = 'PRIMARY';

select * from Users;

SELECT table_name
FROM information_schema.tables
WHERE table_type='BASE TABLE'
      AND table_schema = 'HHDB'