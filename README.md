# QuizApp

Application web full-stack de création, gestion et passation de quiz interactifs, avec support des questions de code, gestion des utilisateurs et des rôles.

---

## Table des matières

- [Présentation](#présentation)
- [Stack technique](#stack-technique)
- [Structure de la solution](#structure-de-la-solution)
- [Fonctionnalités](#fonctionnalités)
- [Architecture & Base de données](#architecture--base-de-données)
- [Prérequis](#prérequis)
- [Installation & Lancement](#installation--lancement)
- [Configuration](#configuration)
- [Endpoints API](#endpoints-api)

---

## Présentation

QuizApp est une plateforme complète permettant à des administrateurs de concevoir des quiz (avec plusieurs types de questions) et à des étudiants de les passer dans un environnement chronométré. Elle inclut un système d'administration avancé pour la gestion des utilisateurs et des rôles, ainsi qu'un éditeur de code intégré pour les questions de programmation.

---

## Stack technique

| Couche | Technologies |
|---|---|
| Frontend | Blazor WebAssembly, MudBlazor (Material Design) |
| Backend | ASP.NET Core 7.0 (2 APIs séparées) |
| Base de données | SQL Server (2 bases séparées) |
| ORM | Entity Framework Core 7 |
| Authentification | JWT Bearer + ASP.NET Identity |
| Temps réel | SignalR |
| Éditeur de code | BlazorMonaco (Monaco Editor) |
| Exécution de code | JDoodle API |
| Mapping | AutoMapper 12 |

---

## Structure de la solution

```
QuizApp/
├── Authentication.web/        # Frontend Blazor WebAssembly
├── authentification_Api/      # API Authentification & Utilisateurs (port 7297)
├── ConceptionQuiz_Api/        # API Quiz & Questions (port 7284)
└── Quiz.Entity/               # Modèles partagés & DTOs
```

---

## Fonctionnalités

### Authentification & Utilisateurs

- **Inscription** : formulaire avec nom, prénom, email, adresse et mot de passe
- **Connexion / Déconnexion** : authentification par JWT, token stocké en local storage
- **Contrôle d'accès par rôle** : Admin et User avec routes protégées côté client et serveur
- **Compte administrateur par défaut** : seedé automatiquement au démarrage (`admin@test.com` / `Admin123!`)

### Gestion des quizzes (Admin)

- **Créer un quiz** : titre, description, niveau de difficulté (1-10), durée (en minutes), nombre de questions, icône
- **Lister les quizzes** : vue grille avec filtrage et pagination
- **Supprimer un quiz** : suppression complète depuis l'interface
- **Assigner un quiz à un utilisateur** : liaison entre un quiz et un étudiant spécifique
- **Visualiser un quiz** : affichage de la structure complète avec toutes ses questions et propositions

### Gestion des questions (Admin)

Trois types de questions sont supportés :

| Type | Description |
|---|---|
| QCM (Multiple Choice) | Plusieurs propositions, une ou plusieurs bonnes réponses |
| Vrai / Faux | Question booléenne simple |
| Code | Question de programmation avec éditeur et exécution |

- Créer une question et choisir son type
- Ajouter/supprimer des propositions dynamiquement
- Marquer les bonnes réponses via des cases à cocher
- Lier des questions existantes à un quiz

### Questions de code

- **Éditeur Monaco intégré** (le même que VS Code) directement dans le navigateur
- **Langages supportés** : C#, Java, C++
- **Exécution en temps réel** via l'API JDoodle
- **Affichage de la sortie** (output) du code exécuté

### Tableau de bord étudiant

- Vue des quizzes disponibles sous forme de cartes (titre, difficulté, durée, nombre de questions)
- Lancement d'un quiz depuis le tableau de bord
- Interface de passation de quiz avec :
  - **Timer** visible et décompte en temps réel
  - Navigation entre les questions
  - Rendu adaptatif selon le type de question (QCM, Vrai/Faux, Code)
  - **Soumission automatique** à l'expiration du temps
- Calcul et affichage du score final

### Administration

- **Gestion des utilisateurs**
  - Tableau avec pagination et recherche
  - Créer, modifier, supprimer un utilisateur
  - Voir le détail d'un utilisateur
- **Gestion des rôles**
  - Créer, modifier, supprimer des rôles
  - Lister tous les rôles existants
- **Assignation des rôles**
  - Interface drag-and-drop pour assigner des rôles aux utilisateurs
  - Possibilité de retirer tous les rôles d'un utilisateur
- **Barre d'actions unifiée** (ManagementBar) : boutons Ajouter, Modifier, Supprimer, Assigner, Détail

---

## Architecture & Base de données

### Deux bases SQL Server séparées

**QuizAuthenticationDB** (API Auth)
- Tables ASP.NET Identity : `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`
- Champs personnalisés sur l'utilisateur : `nom`, `prenom`, `adresse`, `dateCreation`

**QuizConceptionDB** (API Quiz)
- `quiz` — titre, description, difficulté, durée, icône
- `questions` — texte, type (MCQ / TrueFalse / Coding)
- `propositions` — choix de réponse liés à une question
- `reponses` — réponses des utilisateurs, avec flag `IsAnswer` et `output`
- `QuizUser` — table de liaison quiz ↔ utilisateur

### Flux d'authentification JWT

1. L'utilisateur envoie ses identifiants → API Auth valide et génère un JWT
2. Le token est stocké dans le `localStorage` (`tokenAccess`)
3. Chaque requête API ultérieure envoie le token dans le header `Authorization: Bearer`
4. Le frontend parse les claims JWT pour afficher le bon contenu selon le rôle

---

## Prérequis

- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- SQL Server (instance `SQLEXPRESS` par défaut)
- Compte [JDoodle](https://www.jdoodle.com/) (pour l'exécution des questions de code)

---

## Installation & Lancement

```bash
# 1. Restaurer les dépendances
dotnet restore

# 2. Appliquer les migrations (pour chaque API)
cd authentification_Api
dotnet ef database update

cd ../ConceptionQuiz_Api
dotnet ef database update

# 3. Lancer les trois projets (dans des terminaux séparés)
cd authentification_Api && dotnet run      # Port 7297
cd ConceptionQuiz_Api && dotnet run        # Port 7284
cd Authentication.web && dotnet run        # Frontend Blazor
```

---

## Configuration

### `authentification_Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=QuizAuthenticationDB;Integrated Security=True;Encrypt=False"
  },
  "JWT": {
    "ValidIssuer": "https://localhost:7297",
    "ValidAudience": "User",
    "Secret": "VOTRE_CLE_SECRETE"
  }
}
```

### `ConceptionQuiz_Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=QuizConceptionDB;Integrated Security=True;Encrypt=False"
  },
  "JDoodle": {
    "clientId": "VOTRE_CLIENT_ID",
    "clientSecret": "VOTRE_CLIENT_SECRET"
  }
}
```

---

## Endpoints API

### API Authentification — `https://localhost:7297`

| Méthode | Route | Description |
|---|---|---|
| POST | `/api/Compte/auth` | Connexion (retourne un JWT) |
| POST | `/api/Compte/signUp` | Inscription |
| POST | `/api/Compte/logout` | Déconnexion |
| GET | `/api/Administration/ListUsers` | Liste tous les utilisateurs |
| GET | `/api/Administration/ListUsersRoles` | Liste utilisateurs avec leurs rôles |
| GET | `/api/Administration/ListRoles` | Liste tous les rôles |
| GET | `/api/Administration/userById` | Détail d'un utilisateur |
| POST | `/api/Administration/AddUser` | Créer un utilisateur |
| POST | `/api/Administration/AddRole` | Créer un rôle |
| POST | `/api/Administration/updateUser` | Modifier un utilisateur |
| POST | `/api/Administration/updateRole` | Modifier un rôle |
| POST | `/api/Administration/AssignRole` | Assigner un rôle à un utilisateur |
| POST | `/api/Administration/ClearRoles` | Retirer tous les rôles d'un utilisateur |
| DELETE | `/api/Administration/deleteUser` | Supprimer un utilisateur |
| DELETE | `/api/Administration/deleteRole` | Supprimer un rôle |

### API Quiz & Questions — `https://localhost:7284`

| Méthode | Route | Description |
|---|---|---|
| POST | `/api/Quiz/AddQuiz` | Créer un quiz |
| GET | `/api/Quiz/ListQuiz` | Liste tous les quizzes |
| GET | `/api/Quiz/ListQuizByUser` | Quizzes assignés à un utilisateur |
| GET | `/api/Quiz/GetQuizById` | Détail d'un quiz |
| POST | `/api/Quiz/DeleteQuiz` | Supprimer un quiz |
| POST | `/api/Quiz/BindQuizToQuestion` | Ajouter une question à un quiz |
| POST | `/api/Quiz/BindQuizToUser` | Assigner un quiz à un utilisateur |
| POST | `/api/Question/AddQuestion` | Créer une question |
| GET | `/api/Question/ListQuestion` | Liste toutes les questions |
| GET | `/api/Question/GetQuestionsById` | Détail d'une question |
| GET | `/api/Question/GetQuestionsByQuizId` | Questions d'un quiz |
| POST | `/api/Question/GetOutput` | Exécuter du code (JDoodle) |

### Hub SignalR

| Route | Description |
|---|---|
| `/notificationshub` | Hub de messagerie temps réel |
