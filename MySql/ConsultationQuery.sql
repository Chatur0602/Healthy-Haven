use HHDB;

CREATE TABLE Consultations (
  id INT NOT NULL AUTO_INCREMENT,
  description TEXT NOT NULL,
  student_id VARCHAR(255) NOT NULL,
  instructor_id VARCHAR(255) NOT NULL,
  date DATETIME NOT NULL,
  time DATETIME NOT NULL,
  FOREIGN KEY (student_id) REFERENCES AspNetUsers(id),
  FOREIGN KEY (instructor_id) REFERENCES AspNetUsers(id),
  PRIMARY KEY (id)
);

drop table Consultations;