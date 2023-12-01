use HHDB;

CREATE TABLE CourseImages (
  id INT NOT NULL AUTO_INCREMENT,
  course_id INT NOT NULL,
  image_path VARCHAR(255) NOT NULL,
  FOREIGN KEY (course_id) REFERENCES Courses(id),
  PRIMARY KEY (id)
);

drop table CourseImages;

ALTER TABLE CourseImages
ADD CONSTRAINT fk_course
FOREIGN KEY (course_id)
REFERENCES Courses(id)
ON DELETE CASCADE;

select * from CourseImages;
UPDATE CourseImages SET course_id = NULL WHERE course_id = course_id;