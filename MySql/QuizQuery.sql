use HHDB;

/*
 * dropped tables
CREATE TABLE Quizzes (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(255) NOT NULL,
  description TEXT NOT NULL,
  course_id INT NOT NULL,
  FOREIGN KEY (course_id) REFERENCES Courses(id),
  PRIMARY KEY (id)
);

drop table Quizzes;

CREATE TABLE Categories(
	category_id int NOT NULL AUTO_INCREMENT,
    category_name VARCHAR(500) NOT NULL,
    PRIMARY KEY (category_id)
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
*/

-- implementaion based on nothing
CREATE TABLE Quizzes (
  id INT auto_increment,
  title VARCHAR(250) NOT NULL,
  description VARCHAR(500) NOT NULL,
  courseId INT NOT NULL,
  userId VARCHAR(255) NOT NULL,
  date DATE NOT NULL,
  PRIMARY KEY (id),
  FOREIGN KEY (courseId) REFERENCES Courses(id),
  FOREIGN KEY (userId) REFERENCES AspNetUsers(id)
);

Select * FROM Quizzes;

CREATE TABLE Questions (
    id int NOT NULL AUTO_INCREMENT,
    quizId int NOT NULL,
    questionText VARCHAR(500) NOT NULL,
    isActive BIT NOT NULL DEFAULT 1,
    PRIMARY KEY (id),
    FOREIGN KEY (quizId) REFERENCES Quizzes(id)
);

CREATE TABLE Options(
	id int NOT NULL AUTO_INCREMENT,
    questionId INT NOT NULL,
    optionText VARCHAR(500) NOT NULL,
    isCorrect BIT default 0 NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (questionId) REFERENCES Questions(id)
);



-- drop table Questions;


select * from Questions;

-- Dummy data
INSERT INTO Quizzes (title, description, courseId, userId, date) VALUES
('Quiz 1', 'Description for Quiz 1', 1, '0f021eb3-4a7a-4388-b9cb-b4c9ade57012', '2023-11-16'),
('Quiz 2', 'Description for Quiz 2', 2, '0f021eb3-4a7a-4388-b9cb-b4c9ade57012', '2023-11-17'),
('Quiz 3', 'Description for Quiz 3', 3, '0f021eb3-4a7a-4388-b9cb-b4c9ade57012', '2023-11-18'),
('Quiz 4', 'Description for Quiz 4', 2, '0f021eb3-4a7a-4388-b9cb-b4c9ade57012', '2023-11-19');

INSERT INTO Questions (quizId, questionText, isActive) VALUES
(5, 'Question 1 for Quiz 1', 1),
(5, 'Question 2 for Quiz 1', 1),
(6, 'Question 1 for Quiz 2', 1),
(6, 'Question 2 for Quiz 2', 1),
(7, 'Question 1 for Quiz 3', 1),
(7, 'Question 2 for Quiz 3', 1),
(8, 'Question 1 for Quiz 4', 1),
(8, 'Question 2 for Quiz 4', 1);


INSERT INTO Options (questionId, optionText, isCorrect) VALUES
(17, 'Option 1 for Question 1', 1),
(17, 'Option 2 for Question 1', 0),
(17, 'Option 3 for Question 1', 0),
(18, 'Option 1 for Question 2', 0),
(18, 'Option 2 for Question 2', 1),
(18, 'Option 3 for Question 2', 0),
(19, 'Option 1 for Question 3', 1),
(19, 'Option 2 for Question 3', 0),
(19, 'Option 3 for Question 3', 0),
(20, 'Option 1 for Question 4', 0),
(20, 'Option 2 for Question 4', 1),
(20, 'Option 3 for Question 4', 0);




