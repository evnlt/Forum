START TRANSACTION;

INSERT INTO "AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount")
VALUES ('161785c2-91c6-461c-a012-6ffb0676fb57', 'user1@test.com', 'USER1@TEST.COM', 'user1@test.com', 'USER1@TEST.COM', true, 'AQAAAAIAAYagAAAAEJeX82n0naHdxirnDo69aNx/qISFpIgFaRwzjQERJgazK6LkX3IxgqZPumndShQsmA==', 'AHAR57BKR7SADW2UUMFWDHIDWQ7ZVQDK', '30da1d99-5705-4711-8a05-bb0c17c17283', null, false, false, null, true, 0);

INSERT INTO "AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount")
VALUES ('31ddf366-d6d2-46c4-b837-b6f356ee8607', 'user2@test.com', 'USER2@TEST.COM', 'user2@test.com', 'USER2@TEST.COM', true, 'AQAAAAIAAYagAAAAEJeX82n0naHdxirnDo69aNx/qISFpIgFaRwzjQERJgazK6LkX3IxgqZPumndShQsmA==', 'AHAR57BKR7SADW2UUMFWDHIDWQ7ZVQDK', '30da1d99-5705-4711-8a05-bb0c17c17283', null, false, false, null, true, 0);

INSERT INTO "Posts" ("Id", "Title", "Body") VALUES ('bcfc40b2-33e9-40ba-bbe5-1efa2ee3096f', 'Post 1', 'Body text 1...');
INSERT INTO "Posts" ("Id", "Title", "Body") VALUES ('2ea39a1b-d310-4ba1-8a85-12f5fd4a7883', 'Post 2', 'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.');

INSERT INTO "Comments" ("Id", "Message", "CreatedAt", "UpdatedAt", "UserId", "PostId", "ParentId") 
VALUES ('3ddccef1-94bd-44af-9d5d-54195fd3120b', 'Root comment', '2024-02-05 13:16:55.409937', '2024-02-05 14:16:55.409937', '161785c2-91c6-461c-a012-6ffb0676fb57', 'bcfc40b2-33e9-40ba-bbe5-1efa2ee3096f', null);

INSERT INTO "Comments" ("Id", "Message", "CreatedAt", "UpdatedAt", "UserId", "PostId", "ParentId") 
VALUES ('9892d8ff-9f1b-44aa-9002-494967485731', 'Nested comment', '2024-02-05 14:16:55.409937', '2024-02-05 14:16:55.409937', '31ddf366-d6d2-46c4-b837-b6f356ee8607', 'bcfc40b2-33e9-40ba-bbe5-1efa2ee3096f', '3ddccef1-94bd-44af-9d5d-54195fd3120b');

INSERT INTO "Comments" ("Id", "Message", "CreatedAt", "UpdatedAt", "UserId", "PostId", "ParentId") 
VALUES ('ba9e4ac7-839c-46cb-ac27-c3d90b91d164', 'Another root comment', '2024-02-05 14:16:55.409937', '2024-02-05 14:16:55.409937', '31ddf366-d6d2-46c4-b837-b6f356ee8607', '2ea39a1b-d310-4ba1-8a85-12f5fd4a7883', null);

COMMIT;
