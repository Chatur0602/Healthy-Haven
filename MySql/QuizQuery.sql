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

-- implementaion based on video --hopefully the last
CREATE TABLE Categories(
	category_id INT NOT NULL AUTO_INCREMENT,
	category_name VARCHAR(50) NOT NULL,
    PRIMARY KEY (category_id)
);

create table Quiz_questions(
	question_id INT NOT NULL AUTO_INCREMENT,
    category_id INT NOT NULL,
    question_name VARCHAR(500) NOT NULL,
    PRIMARY KEY (question_id),
    FOREIGN KEY (category_id) REFERENCES Categories(category_id)
);

CREATE TABLE Quiz_Options(
	option_id INT NOT NULL AUTO_INCREMENT,
    option_name VARCHAR(500) NOT NULL,
    PRIMARY KEY (option_id)
);

CREATE TABLE Answer(
	answer_id INT NOT NULL AUTO_INCREMENT,
    question_id INT NOT NULL,
    answer_text VARCHAR(500) NOT NULL,
    PRIMARY KEY (answer_id),
    FOREIGN KEY (question_id) REFERENCES Quiz_questions(question_id)
    );
    
CREATE TABLE Results(
	result_id INT NOT NULL AUTO_INCREMENT,
    user_id VARCHAR(255) NOT NULL,
	question_id INT NOT NULL,
    answer_id INT NOT NULL,
    answer_text VARCHAR(500) NOT NULL,
    PRIMARY KEY (result_id),
    FOREIGN KEY (user_id) REFERENCES AspNetUsers(id),
    FOREIGN KEY (question_id) REFERENCES Quiz_questions(question_id)
    
);


DROP TABLE Categories;
DROP TABLE Quizz_questions;
DROP TABLE Options;
DROP TABLE Answer;
DROP TABLE Results;



