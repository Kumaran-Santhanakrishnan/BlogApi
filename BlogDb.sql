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
    
