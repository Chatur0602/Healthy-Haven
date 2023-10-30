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

Select * From Forums;

drop table Forums;

Insert Into Forums(id, title, description, user_id, created_at)values('1', 'TestForum','checkin this out', '26200b4a-c0e9-46ad-ae50-6f0e3c53c4fc', '2023-10-10');