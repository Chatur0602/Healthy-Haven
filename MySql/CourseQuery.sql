use HHDB;

CREATE TABLE Courses (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
  description TEXT NOT NULL,
  course_duration time,
  credit_hours decimal(5,2),
  instructor_id VARCHAR(255) NOT NULL,
  FOREIGN KEY (instructor_id) REFERENCES AspNetUsers(Id),
  PRIMARY KEY (id)
);

drop table Courses;