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

Select * FROM QuizResponses;

CREATE TABLE Categories(
	category_id int NOT NULL AUTO_INCREMENT,
    category_name VARCHAR(500) NOT NULL,
    PRIMARY KEY (category_id)
);

CREATE TABLE Questions (
    question_id int NOT NULL AUTO_INCREMENT,
    category_id int NOT NULL,
    question_name VARCHAR(500) NOT NULL,
    isActive BIT NOT NULL DEFAULT 1,
    isMultiple BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (question_id),
    FOREIGN KEY (category_id) REFERENCES Categories(category_id)
);

CREATE TABLE Options(
	option_id int NOT NULL AUTO_INCREMENT,
    question_id INT NOT NULL,
    option_name VARCHAR(500) NOT NULL,
    PRIMARY KEY (option_id),
    FOREIGN KEY (question_id) REFERENCES Questions(question_id)
);

CREATE TABLE Answers(
	answer_id int NOT NULL AUTO_INCREMENT,
    question_id INT NOT NULL,
    answer_text VARCHAR(500) NOT NULL,
    PRIMARY KEY (answer_id),
    FOREIGN KEY (question_id) REFERENCES Questions(question_id)
);

CREATE TABLE Results(
	result_id INT NOT NULL AUTO_INCREMENT,
    user_id VARCHAR(255) NOT NULL,
    question_id INT NOT NULL,
    answer_text VARCHAR(500) NOT NULL,
    PRIMARY KEY (result_id),
    FOREIGN KEY (question_id) REFERENCES Questions(question_id),
    FOREIGN KEY (user_id) REFERENCES AspNetUsers(id)
);

drop table Questions;
select * from Questions;






