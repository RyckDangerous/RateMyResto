# Règles T-SQL — SQL Server

> Ces règles s'appliquent à tous les scripts SQL du projet (migrations DbUp, procédures stockées, requêtes ad hoc).
> Elles extraient et centralisent le contenu SQL du `CLAUDE.md` racine.

---

## 🔡 Mots-clés SQL en MAJUSCULE

**TOUJOURS** écrire les mots-clés SQL Server en MAJUSCULE.

Exemples : `SELECT`, `FROM`, `WHERE`, `INSERT`, `UPDATE`, `DELETE`, `JOIN`, `INNER JOIN`, `LEFT JOIN`, `CREATE`, `ALTER`, `DROP`, `TABLE`, `PROCEDURE`, `INDEX`, `CONSTRAINT`, `REFERENCES`, `UNIQUE`, `PRIMARY KEY`, `FOREIGN KEY`, `NOT NULL`, `IDENTITY`, `GO`, etc.

---

## 🔲 Utilisation des crochets `[ ]`

- **NE PAS** mettre des crochets sur tous les champs
- **UNIQUEMENT** pour les noms qui sont des mots-clés SQL réservés

```sql
-- ✅ BON - Crochets uniquement sur les mots-clés SQL
[Description], [Role], [User], [Group], [Order], [Key], [Type]

-- ❌ MAUVAIS - Crochets sur tout
[Id], [Name], [TeamId], [CreatedAt]
```

---

## 🚫 Interdiction des contraintes `DEFAULT`

**INTERDICTION TOTALE** : Ne jamais utiliser de contraintes `DEFAULT` sur les colonnes.
Les valeurs par défaut doivent être gérées au niveau de l'application.

```sql
-- ❌ MAUVAIS
CREATE TABLE dbo.Event
(
    Id        INT IDENTITY(1,1) NOT NULL,
    Name      NVARCHAR(200)     NOT NULL,
    CreatedAt DATETIME2         NOT NULL DEFAULT GETUTCDATE(),
    IsActive  BIT               NOT NULL DEFAULT 1
);

-- ✅ BON - Valeurs gérées par l'application
CREATE TABLE dbo.Event
(
    Id        INT IDENTITY(1,1) NOT NULL,
    Name      NVARCHAR(200)     NOT NULL,
    CreatedAt DATETIME2         NOT NULL,
    IsActive  BIT               NOT NULL
);
```

---

## 🗂️ Schéma obligatoire

**TOUJOURS** préfixer les noms de tables avec le schéma : `dbo.`

```sql
-- ✅ BON
SELECT * FROM dbo.Event AS e;
INSERT INTO dbo.ReminderSent (EventRepasId, UserTeamsId, SentAt) VALUES (...);

-- ❌ MAUVAIS
SELECT * FROM Event AS e;
INSERT INTO ReminderSent (...) VALUES (...);
```

---

## 🏷️ Convention de nommage des contraintes

| Type | Format | Exemple |
|------|--------|---------|
| **Primary Key** | `PK_{NomTable}` | `PK_Event` |
| **Foreign Key** | `FK_{NomTable}_{NomTableReferee}` | `FK_Event_Team` |
| **Unique** | `UK_{NomTable}_{NomColonne}` | `UK_ReminderSent_Event_User` |
| **Check** | `CK_{NomTable}_{NomColonne}` | `CK_Event_Status` |
| **Index** | `IX_{NomTable}_{NomColonne}` | `IX_ReminderSent_EventRepasId` |

```sql
-- ✅ BON
CREATE TABLE dbo.Event
(
    Id     INT IDENTITY(1,1) NOT NULL,
    Name   NVARCHAR(200)     NOT NULL,
    TeamId INT               NOT NULL,

    CONSTRAINT PK_Event
        PRIMARY KEY (Id),

    CONSTRAINT FK_Event_Team
        FOREIGN KEY (TeamId) REFERENCES dbo.Team(Id)
);

CREATE INDEX IX_Event_TeamId
    ON dbo.Event (TeamId);
```

---

## 📐 Indentation et lisibilité

### Requêtes SELECT

```sql
-- ✅ BON - Colonnes alignées, conditions indentées
SELECT e.Id,
       e.Name,
       t.Name AS TeamName
FROM dbo.Event AS e
INNER JOIN dbo.Team AS t
    ON e.TeamId = t.Id
WHERE e.TeamId = @TeamId
  AND e.Date >= @DateStart
  AND e.IsActive = 1;

-- ❌ MAUVAIS - Tout sur une ligne
SELECT e.Id, e.Name FROM dbo.Event AS e WHERE e.TeamId = @TeamId AND e.IsActive = 1;
```

### Conditions alignées

- Opérateurs `AND` / `OR` en début de ligne, alignés sous `WHERE`
- Chaque condition sur sa propre ligne dès que plus d'une condition

```sql
-- ✅ BON
WHERE e.DateEvenement < CAST(GETDATE() AS DATE)
  AND p.StatusParticipationId = 2
  AND p.Note IS NULL
  AND usr.Email IS NOT NULL

-- ❌ MAUVAIS
WHERE e.DateEvenement < CAST(GETDATE() AS DATE) AND p.StatusParticipationId = 2 AND p.Note IS NULL
```

### Procédures stockées

```sql
-- ✅ BON - Structure d'une procédure stockée
CREATE PROCEDURE sp_NomProcedure
    @Param1 TYPE,
    @Param2 TYPE
AS
BEGIN
    SET NOCOUNT ON;

    -- Corps de la procédure
END
GO
```

---

## 📌 Résumé : Les règles d'or T-SQL

1. ✅ **Mots-clés en MAJUSCULE** — SELECT, FROM, WHERE, CREATE, etc.
2. ✅ **Crochets `[ ]` uniquement sur les mots-clés SQL réservés** — [Description], [Role], [Order]…
3. ✅ **INTERDICTION DEFAULT** — pas de contraintes DEFAULT sur les colonnes, valeurs gérées par l'application
4. ✅ **Schéma obligatoire** — toujours `dbo.TableName`
5. ✅ **Contraintes nommées** — PK_, FK_, UK_, CK_, IX_
6. ✅ **Indentation claire** — aligner les colonnes SELECT, mettre AND en début de ligne sous WHERE
7. ✅ **Alias significatifs** — courts mais compréhensibles (ex: `e` pour `Event`, `t` pour `Team`)

---

💡 **Note** : Ces règles s'appliquent aussi bien aux scripts de migration DbUp qu'aux procédures stockées créées dans les scripts.
