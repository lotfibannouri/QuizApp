using QuizApp.Entities.Base_Entities;
using System;

namespace QuizApp.Entities.Conception_Entities
{
    /// <summary>
    /// Détail d'un résultat, question par question. Texte, type, catégorie et barème sont
    /// copiés au moment du passage, pour les mêmes raisons que dans Resultat.
    /// </summary>
    public class ResultatQuestion : BaseEntity
    {
        public Guid resultatId { get; set; }
        public Resultat? resultat { get; set; }

        /// <summary>Nul si la question a été supprimée depuis.</summary>
        public Guid? questionId { get; set; }
        public Question? question { get; set; }

        /// <summary>Position de la question dans le test.</summary>
        public int ordre { get; set; }

        public string questionTexte { get; set; }

        /// <summary>Choix Multiple, Vrai/Faux ou Codage.</summary>
        public string questionType { get; set; }

        /// <summary>Catégorie de la question au moment du passage (copie, sans clé étrangère).</summary>
        public Guid? categorieId { get; set; }

        public double score { get; set; }
        public double scoreMax { get; set; }
        public bool isCorrecte { get; set; }

        /// <summary>Distingue une réponse fausse d'une absence de réponse.</summary>
        public bool isRepondue { get; set; }

        /// <summary>Réponse de l'étudiant : JSON des réponses cochées, ou code soumis.</summary>
        public string? reponseDonnee { get; set; }

        /// <summary>Sortie du code exécuté (questions Codage).</summary>
        public string? sortieObtenue { get; set; }
    }
}
