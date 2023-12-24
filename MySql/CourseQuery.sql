use HHDB;

CREATE TABLE Courses (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
  description TEXT NOT NULL,
  course_duration DATETIME NOT NULL,
  credit_hours decimal(5,2),
  instructor_id VARCHAR(255) NOT NULL,
  FOREIGN KEY (instructor_id) REFERENCES AspNetUsers(Id),
  PRIMARY KEY (id)
);

drop table Courses;

DELETE FROM Courses where id = 1;
select * from Courses;

CREATE TABLE Modules (
  id INT NOT NULL AUTO_INCREMENT,
  chapter VARCHAR(255) NOT NULL,
  module TEXT NOT NULL,
  course_id INT NOT NULL,
  FOREIGN KEY (course_id) REFERENCES Courses(id),
  PRIMARY KEY (id)
);

drop table Modules;
ALTER TABLE Modules
drop COLUMN chapter; 
select * from Modules;


CREATE TABLE Chapters (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
content text Not Null,
module_id INT NOT NULL,
  FOREIGN KEY (module_id) REFERENCES Modules(id),
  PRIMARY KEY (id)
);
insert into Chapters (name,content,module_id) values ("Chapter 1: Build Chest","For Building Biceps perform arm XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", 1);
select * from Chapters;
