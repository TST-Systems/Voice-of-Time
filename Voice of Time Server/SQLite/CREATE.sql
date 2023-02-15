﻿-- ----- ----- TABLES ----- ----- --
-- ID Register for Users, Chats and CHannels
CREATE TABLE IF NOT EXISTS IDS (
    id        INTEGER PRIMARY KEY,
    isUser    BOOLEAN NOT NULL DEFAULT false,
    isChat    BOOLEAN NOT NULL DEFAULT false,
    isChannel BOOLEAN NOT NULL DEFAULT false
);

CREATE TABLE IF NOT EXISTS USERS (
    id        INTEGER PRIMARY KEY,
    username  TEXT NOT NULL,
    PublicKey TEXT NOT NULL,
    FOREIGN KEY(id) REFERENCES IDS(id)
);

CREATE TABLE IF NOT EXISTS CHATS (
    id        INTEGER PRIMARY KEY,
    FOREIGN KEY(id) REFERENCES IDS(id)
);

CREATE TABLE IF NOT EXISTS CHANNELS (
    id        INTEGER PRIMARY KEY,
    name      TEXT NOT NULL,
    PublicKey TEXT NOT NULL,
    FOREIGN KEY(id) REFERENCES IDS(id)
);

CREATE TABLE IF NOT EXISTS CHAT_MEMBERS (
    chat_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    state   INTEGER NOT NULL,
    FOREIGN KEY(chat_id) REFERENCES CHATS(id),
    FOREIGN KEY(user_id) REFERENCES USERS(id),
    PRIMARY KEY(chat_id, user_id)
);

-- ----- ----- TRIGGER ----- ----- --
-- INSERT
CREATE TRIGGER IF NOT EXISTS INSERT_USERS 
BEFORE INSERT ON USERS FOR EACH ROW 
BEGIN
	INSERT INTO IDS (id, isuser) VALUES (NEW.id, true);
END;

CREATE TRIGGER IF NOT EXISTS INSERT_CHATS
BEFORE INSERT ON CHATS FOR EACH ROW
BEGIN
    INSERT INTO IDS (id, ischat) VALUES (NEW.id, true);
END;

CREATE TRIGGER IF NOT EXISTS INSERT_CHANNELS
BEFORE INSERT ON CHANNELS FOR EACH ROW
BEGIN
    INSERT INTO IDS (id, ischannel) VALUES (NEW.id, true);
END;
-- UPDATE
CREATE TRIGGER IF NOT EXISTS UPDATE_USERS
BEFORE UPDATE ON USERS FOR EACH ROW
BEGIN
    UPDATE IDS SET id = NEW.id WHERE id = OLD.id;
END;

CREATE TRIGGER IF NOT EXISTS UPDATE_CHATS
BEFORE UPDATE ON CHATS FOR EACH ROW
BEGIN
    UPDATE IDS SET id = NEW.id WHERE id = OLD.id;
END;

CREATE TRIGGER IF NOT EXISTS UPDATE_CHANNELS
BEFORE UPDATE ON CHANNELS FOR EACH ROW
BEGIN
    UPDATE IDS SET id = NEW.id WHERE id = OLD.id;
END;
-- DELETE
CREATE TRIGGER IF NOT EXISTS DELETE_USERS
BEFORE DELETE ON USERS FOR EACH ROW
BEGIN
    DELETE FROM IDS WHERE id = OLD.id;
END;

CREATE TRIGGER IF NOT EXISTS DELETE_CHATS
BEFORE DELETE ON CHATS FOR EACH ROW
BEGIN
    DELETE FROM IDS WHERE id = OLD.id;
END;

CREATE TRIGGER IF NOT EXISTS DELETE_CHANNELS
BEFORE DELETE ON CHANNELS FOR EACH ROW
BEGIN
    DELETE FROM IDS WHERE id = OLD.id;
END;