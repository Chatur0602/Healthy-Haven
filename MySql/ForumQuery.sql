use HHDB;

CREATE TABLE Forums (
  id INT NOT NULL AUTO_INCREMENT,
  title VARCHAR(255) NOT NULL,
  description TEXT NOT NULL,
  user_id VARCHAR(255) NOT NULL,
  created_at DATETIME NOT NULL,
  FOREIGN KEY (user_id) REFERENCES AspNetUsers(id),
  PRIMARY KEY (id)
);

CREATE TABLE ForumImages (
  id INT NOT NULL AUTO_INCREMENT,
  forum_id INT NOT NULL,
  image_path VARCHAR(255) NOT NULL,
  FOREIGN KEY (forum_id) REFERENCES Forums(id),
  PRIMARY KEY (id)
);

CREATE TABLE Comments (
    Id INT NOT NULL AUTO_INCREMENT,
    CommentText VARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    ForumId INT NOT NULL,
    UserId VARCHAR(255) NOT NULL, 
    PRIMARY KEY (Id),
    FOREIGN KEY (ForumId) REFERENCES Forums(Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

CREATE TABLE CommentLikes (
    Id INT NOT NULL AUTO_INCREMENT,
    UserId VARCHAR(255) NOT NULL,
    CommentId INT, 
    PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (CommentId) REFERENCES Comments(Id)
);

CREATE TABLE ForumLikes (
    Id INT NOT NULL AUTO_INCREMENT,
    UserId VARCHAR(255) NOT NULL,
    ForumId INT,
    PRIMARY KEY (Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (ForumId) REFERENCES Forums(Id)
);

delete from CommentLikes;
delete from Comments;
delete from ForumImages;
delete from ForumLikes;
delete from Forums;

Select * From Forums;
Select * From ForumImages;
Select * From Comments;
Select * From CommentLikes;
Select * From ForumLikes;

DELETE FROM Forums;
DELETE FROM ForumImages;
DELETE FROM CommentLikes ;
DELETE FROM ForumLikes ;

drop table Forums;
drop table Likes;
drop table Comments;

Insert into ForumImages(id, forum_id, image_path)values('1','1','Random Image');
Insert into Comments(Id, CommentText, CreatedAt, ForumId, UserId)values(1,'Random Comment', '2023-10-10',78,'859c0b00-5804-4787-945a-b4794b3f168a');
Insert Into Forums(id, title, description, user_id, created_at)values('1', 'TestForum','checkin this out', '394d9c36-f677-495d-af2f-083dd88cc34c', '2023-10-10');