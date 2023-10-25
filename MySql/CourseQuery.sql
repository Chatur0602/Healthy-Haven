use HHDB;

CREATE TABLE Courses (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
  description TEXT NOT NULL,
  instructor_id INT NOT NULL,
  FOREIGN KEY (instructor_id) REFERENCES Users(id),
  PRIMARY KEY (id)
);

drop table Courses;