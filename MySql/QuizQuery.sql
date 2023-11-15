use HHDB;

/*
CREATE TABLE Quizzes (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
  description TEXT NOT NULL,
  course_id INT NOT NULL,
  FOREIGN KEY (course_id) REFERENCES Courses(id),
  PRIMARY KEY (id)
);

drop table Quizzes;
*/

-- implementaion based on nothing
CREATE TABLE Quizzes (
  quizID INT auto_increment,
  name VARCHAR(500) NOT NULL,
  description VARCHAR(500) NOT NULL,
  authorID VARCHAR(255) NOT NULL,
  date DATE NOT NULL,
  quiz_data JSON NOT NULL,
  PRIMARY KEY (quizID),
  FOREIGN KEY (authorID) REFERENCES AspNetUsers(id)
);

Select * FROM Quizzes;




