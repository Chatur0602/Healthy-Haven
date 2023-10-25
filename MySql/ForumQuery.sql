use HHDB;

CREATE TABLE Forum (
  id INT NOT NULL AUTO_INCREMENT,
  title VARCHAR(255) NOT NULL,
  description TEXT NOT NULL,
  user_id VARCHAR(255) NOT NULL,
  created_at DATETIME NOT NULL,
  FOREIGN KEY (user_id) REFERENCES AspNetUsers(id),
  PRIMARY KEY (id)
);

drop table Forum