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



drop table Questions;


select * from Quizzes;

insert into Categories (category_name) values ("Nutrition");
insert into Questions (category_id, question_name) values (1, "How many calories in an apple?");





