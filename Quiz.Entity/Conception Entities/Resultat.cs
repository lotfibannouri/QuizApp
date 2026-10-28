using QuizApp.Entities.Base_Entities;
using System;
using System.Collections.Generic;

namespace QuizApp.Entities.Conception_Entities
{
    /// <summary>
    /// Résultat d'un passage de quiz. Les valeurs du quiz utiles aux statistiques (titre,
    /// catégorie, durée, barème) sont copiées au moment du passage : un quiz peut être
    /// modifié ou supprimé ensuite sans fausser l'historique.
    /// </summary>
    public class Resultat : BaseEntity
    {
        /// <summary>Nul si le quiz a été supprimé depuis (le résultat est conservé).</summary>
        public Guid? quizid { get; set; }
        public Quiz? quiz { get; set; }
        public string quizTitre { get; set; }

        /// <summary>Catégorie du quiz au moment du passage.</summary>
        public Guid? categorieId { get; set; }
        public Categorie? categorie { get; set; }

        /// <summary>Étudiant : lien logique vers AspNetUsers, qui vit dans une autre base.</summary>
        public Guid userid { get; set; }

        /// <summary>Passage : même identifiant que les proctoringEvents de ce test.</summary>
        public Guid sessionId { get; set; }

        /// <summary>1, 2, 3… pour un même couple quiz / étudiant.</summary>
        public int tentative { get; set; }

        public DateTime startedAt { get; set; }
        public DateTime submittedAt { get; set; }
        public int dureeSecondes { get; set; }
        public int dureeAutoriseeMinutes { get; set; }

        /// <summary>Soumis automatiquement à l'expiration du minuteur.</summary>
        public bool isAutoSubmitted { get; set; }

        public double score { get; set; }
        public double scoreMax { get; set; }
        public double pourcentage { get; set; }
        public string mention { get; set; }
        public bool isReussi { get; set; }

        public int nbQuestions { get; set; }

        /// <summary>Questions obtenues avec la note maximale.</summary>
        public int nbCorrectes { get; set; }

        public ICollection<ResultatQuestion>? questions { get; set; }
    }
}
