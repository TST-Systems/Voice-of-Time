-- TABLES
DROP TABLE IF EXISTS CHAT_MEMBERS;
DROP TABLE IF EXISTS USERS;
DROP TABLE IF EXISTS CHATS;
DROP TABLE IF EXISTS CHANNELS;
DROP TABLE IF EXISTS IDS;
-- TRIGGERS
DROP TRIGGER IF EXISTS INSERT_USERS;
DROP TRIGGER IF EXISTS INSERT_CHATS;
DROP TRIGGER IF EXISTS INSERT_CHANNELS;

DROP TRIGGER IF EXISTS DELETE_USERS;
DROP TRIGGER IF EXISTS DELETE_CHATS;
DROP TRIGGER IF EXISTS DELETE_CHANNELS;

DROP TRIGGER IF EXISTS UPDATE_USERS;
DROP TRIGGER IF EXISTS UPDATE_CHATS;
DROP TRIGGER IF EXISTS UPDATE_CHANNELS;