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

DELETE FROM Courses where id = 77;
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

insert into Modules (module, course_id) values ("Module Build Chest 3", 74);

CREATE TABLE Chapters (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
content text Not Null,
module_id INT NOT NULL,
  FOREIGN KEY (module_id) REFERENCES Modules(id),
  PRIMARY KEY (id)
);

insert into Chapters (name,content,module_id) values ("Chapter 3: Build Chest","For Building Biceps perform arm XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", 13);
insert into Chapters (name,content,module_id) values ("Chapter 3: Build Chest","For Building Biceps perform arm XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", 14);
insert into Chapters (name,content,module_id) values ("Chapter 4: Build Chest","For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm For Building Biceps perform arm ", 15);
select * from Chapters;

CREATE TABLE CoursesEnrolled (
  id INT NOT NULL AUTO_INCREMENT,
  course_id INT NOT NULL,
  user_id VARCHAR(255) NOT NULL,
  FOREIGN KEY (course_id) REFERENCES Courses(id),
  FOREIGN KEY (user_id) REFERENCES AspNetUsers(Id),
  PRIMARY KEY (id)
);

select * from CoursesEnrolled;

delete from CoursesEnrolled where course_id = 77;
