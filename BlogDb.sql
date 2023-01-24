use blog;
create table if not exists user (Id varchar(50),
	Name text,
    Email text,
    ProfileId  text,
    primary key (id)) ;

create table if not exists auth (UserId varchar(50),
	Password text,
    primary key (Userid)) ;

create table if not exists AuthToken (Token varchar(500),
	UserId text,
    CreatedAt Datetime DEFAULT current_timestamp,
    Validity DateTime ,
    primary key (Token)) ;
    
create table if not exists Post (PostId varchar(100),
	AuthorId text,
    content text,
    createdAt DateTime DEFAULT current_timestamp,
    isPublic bool,
    primary key(PostId));
    
create table if not exists Upvotes (Id varchar(100),
	actorId varchar(100),
    PostId varchar(100),
    createdAt DateTime DEFAULT current_timestamp,
    primary key(actorId,PostId),
    foreign key (actorId) REFERENCES user(Id),
    foreign key (PostId) REFERENCES Post(PostId));

ALTER TABLE auth
	ADD COLUMN Salt TEXT AFTER Password;
    
ALTER TABLE Post
	ADD COLUMN title TEXT AFTER AuthorId;
