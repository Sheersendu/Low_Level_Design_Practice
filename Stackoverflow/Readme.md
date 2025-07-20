## API: Here’s a complete REST API summary for your StackOverflow-like system, incorporating all the decisions and improvements you made:

---

### ✅ **Authentication**

* **Assumption:** `userId` is derived from the JWT token in headers for all APIs.

---

### ✅ **📝 Question APIs**

| Method | Endpoint                       | Request Body / Params                                  | Description                                      |
| ------ | ------------------------------ | ------------------------------------------------------ | ------------------------------------------------ |
| POST   | `/api/v1/question`             | `{ "Title": "", "Content": "", "BestAnswerID": null }` | Create a new question                            |
| PUT    | `/api/v1/question/:questionId` | `{ "Title": "", "Content": "", "BestAnswerID": "" }`   | Update title, content, or mark best answer       |
| GET    | `/api/v1/question/:questionId` | –                                                      | Get a specific question with all its answers     |
| DELETE | `/api/v1/question/:questionId` | –                                                      | Deletes a question (cascade delete related data) |

---

### ✅ **💡 Answer APIs**

| Method | Endpoint                   | Request Body                          | Description                  |
| ------ | -------------------------- | ------------------------------------- | ---------------------------- |
| POST   | `/api/v1/answer`           | `{ "QuestionID": "", "Content": "" }` | Post an answer to a question |
| PUT    | `/api/v1/answer/:answerId` | `{ "Content": "" }`                   | Edit an answer               |
| DELETE | `/api/v1/answer/:answerId` | –                                     | Delete an answer             |

---

### ✅ **💬 Comment APIs**

| Method | Endpoint                                  | Request Body                                                                       | Description                                             |
| ------ | ----------------------------------------- | ---------------------------------------------------------------------------------- | ------------------------------------------------------- |
| POST   | `/api/v1/comment`                         | `{ "Text": "", "TargetID": "", "TargetType": "Ans/Que", "ParentCommentID": null }` | Comment on a question, answer, or another comment       |
| PUT    | `/api/v1/comment/:commentId`              | `{ "Text": "" }`                                                                   | Edit a comment                                          |
| DELETE | `/api/v1/comment/:commentId`              | –                                                                                  | Delete a comment (and its nested replies)               |
| GET    | `/api/v1/comments/:targetId?type=Ans/Que` | –                                                                                  | Get all comments for a target in `updatedAt desc` order |

---

### ✅ **📊 Vote APIs**

| Method | Endpoint                 | Request Body                                                         | Description                                |
| ------ | ------------------------ | -------------------------------------------------------------------- | ------------------------------------------ |
| POST   | `/api/v1/vote`           | `{ "TargetID": "", "TargetType": "Ans/Que", "VoteType": "UP/DOWN" }` | Vote on an answer or question (idempotent) |
| GET    | `/api/v1/vote/:targetId` | –                                                                    | Fetch vote count for the given target      |

---

### ✅ **🔐 Rate Limiting (Infra-Level)**

* 10 upvotes per minute per user using Redis-based token bucket.
* Return `429 Too Many Requests` if rate limit exceeded.

---

### ✅ **🧹 Deletion Rules**

* **Cascade Delete**: Deleting a post deletes associated answers, comments, and votes.
* **No Undo**: No soft delete used to conserve space and simplify logic.

---

## DB Design:

Here’s how the **updated schema** would look with minimal changes:

---

### ✅ **Users**

```sql
User (
  id UUID PRIMARY KEY,
  name VARCHAR(100),
  email VARCHAR(100),
  isActive BOOLEAN,
  createdAt TIMESTAMP,
  updatedAt TIMESTAMP
)
```

---

### ✅ **Posts (Questions & Answers split OR unified by type)**

```sql
Post (
  id UUID PRIMARY KEY,
  userId UUID REFERENCES User(id),
  title VARCHAR(255),         -- Nullable for answers
  content TEXT,
  type ENUM('QUESTION', 'ANSWER'),
  parentId UUID,              -- NULL for questions, questionId for answers
  bestAnswerId UUID,          -- Nullable, only used for questions
  createdAt TIMESTAMP,
  updatedAt TIMESTAMP
)
```

> ✅ `bestAnswerId` lives in the **question row** and points to the **answer's postId**.

---

### ✅ **Comments**

```sql
Comment (
  id UUID PRIMARY KEY,
  text TEXT,
  userId UUID REFERENCES User(id),
  targetId UUID,              -- Post ID or Comment ID
  targetType ENUM('POST', 'COMMENT'),
  parentCommentId UUID,       -- For nesting comments
  createdAt TIMESTAMP,
  updatedAt TIMESTAMP
)
```

---

### ✅ **Votes**

```sql
Vote (
  id UUID PRIMARY KEY,
  userId UUID REFERENCES User(id),
  targetId UUID,              -- Post ID only
  voteType ENUM('UP', 'DOWN'),
  createdAt TIMESTAMP,
  UNIQUE(userId, targetId)    -- Enforces one vote per user per post
)
```

---

### ✅ **Feedback**

```sql
Feedback (
  id UUID PRIMARY KEY,
  postId UUID REFERENCES Post(id),
  userId UUID REFERENCES User(id),
  feedbackType ENUM('LIKE', 'DISLIKE', 'REPORT'),
  createdAt TIMESTAMP
)
```

---

These timestamps will support:

* Ordering (e.g., most recently updated comments first)
* Auditing (for moderation, debugging)
* Future features like "edited recently" or "trending" answers

---