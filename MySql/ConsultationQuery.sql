use HHDB;

CREATE TABLE Consultations (
  id INT NOT NULL AUTO_INCREMENT,
  description TEXT NOT NULL,
  student_id INT NOT NULL,
  instructor_id INT NOT NULL,
  date DATETIME NOT NULL,
  time DATETIME NOT NULL,
  FOREIGN KEY (student_id) REFERENCES Users(id),
  FOREIGN KEY (instructor_id) REFERENCES Users(id),
  PRIMARY KEY (id)
);

drop table Consultations;