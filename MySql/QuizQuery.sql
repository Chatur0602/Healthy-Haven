use HHDB;

CREATE TABLE Quizzes (
    quizID INT NOT NULL AUTO_INCREMENT,
    title VARCHAR(50) NOT NULL,
    description VARCHAR(255) NOT NULL,
    authorID VARCHAR(255) NOT NULL,
    date DATE NOT NULL,
    subject VARCHAR(50),
    FOREIGN KEY (authorID) REFERENCES AspNetUsers(id),
    PRIMARY KEY (quizID)
);

CREATE TABLE Questions (
	questionID int NOT NULL AUTO_INCREMENT,
	quizID VARCHAR(255) NOT NULL,
	questionText VARCHAR(255) NOT NULL,
	questionType VARCHAR(8) NOT NULL,
	multipleOptions JSON NOT NULL,
	FOREIGN KEY (quizID) REFERENCES AspNetUsers(id),
	PRIMARY KEY (questionID)
);

CREATE TABLE QuizResponses (
	responseID int AUTO_INCREMENT,
	userID VARCHAR(255) NOT NULL,
	questionID INT NOT NULL,
	userAnswers JSON NOT NULL,
	pointsEarned INT NOT NULL,
	FOREIGN KEY (userID) REFERENCES AspNetUsers(id),
	FOREIGN KEY (questionID) REFERENCES Questions(questionID),
	PRIMARY KEY (responseID)
);

-- THE USER ID FOR THE QuizResponses is for registered users / authorID is meant for Admin User, 
-- drop table Quizzes;