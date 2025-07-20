Here’s a complete REST API summary for your StackOverflow-like system, incorporating all the decisions and improvements you made:

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