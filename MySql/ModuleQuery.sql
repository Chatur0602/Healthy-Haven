use HHDB;

CREATE TABLE Modules (
  id INT NOT NULL AUTO_INCREMENT,
  chapter VARCHAR(255) NOT NULL,
  module TEXT NOT NULL,
  course_id INT NOT NULL,
  FOREIGN KEY (course_id) REFERENCES Courses(id),
  PRIMARY KEY (id)
);

drop table Modules;

insert into Modules (chapter,module,course_id) values ("Chapter 1: Build bicep section","Module 1.1: Front Lift", 58);
ALTER TABLE Modules
ADD COLUMN chapters varchar(255);

select * from Modules