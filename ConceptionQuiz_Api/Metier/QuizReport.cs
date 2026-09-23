using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;

namespace ConceptionQuiz_Api.Metier
{
    public class QuizReport : IQuizReport
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizRepository _quizRepository;
        public QuizReport(IQuestionRepository questionRepository, IQuizRepository quizRepository)
        {
            _questionRepository = questionRepository;
            _quizRepository = quizRepository;
        }

        public async Task<double> CalculateMultiChoiceScore(ScoreRequestDTO request)
        {
            var question = await _questionRepository.GetQuestionById(request.QuestionId);
            if (question?.reponses == null)
                return 0;

            bool hasCorrectChecked = question.reponses.Any(mapped =>
                mapped.IsAnswer && request.Answers.Any(a => a.Body == mapped.Body && a.IsChecked));
            if (!hasCorrectChecked)
                return 0;

            double score = 0;
            foreach (var mapped in question.reponses)
            {
                double coefficient = (double)question.note / question.propositions.Count;
                bool isChecked = request.Answers.Any(a => a.Body == mapped.Body && a.IsChecked);
                bool isMatch = mapped.IsAnswer == isChecked;

                if (hasCorrectChecked)
                    score += isMatch ? coefficient : -coefficient;
                //else if (isChecked && !mapped.IsAnswer)
                //    score -= coefficient;
            }

            return score;
        }

        //public async Task<byte[]> GenerateQuizReportAsync(QuizReportRequestDTO request)
        //{
        //    try
        //    {
        //        var quiz = await _quizRepository.GetQuizById(request.QuizId);
        //        byte[] pdfBytes = Document.Create(container =>
        //        {
        //            container.Page(async page =>
        //            {
        //                page.Size(PageSizes.A4);
        //                page.Margin(2, Unit.Centimetre);
        //                page.PageColor(Colors.White);
        //                page.DefaultTextStyle(x => x.FontSize(20));

        //                page.Header()
        //                    .Text("Rapport de score : " + quiz.titre)
        //                    .SemiBold()
        //                    .FontSize(36)
        //                    .FontColor(Colors.Blue.Medium);

        //                page.Content()
        //                    .PaddingVertical(1, Unit.Centimetre)
        //                    .Row(async x =>
        //                    {


        //                        double total = 0;
        //                        foreach (var q in request.ListScoreRequest)
        //                        {
        //                            var question = await _questionRepository.GetQuestionById(q.QuestionId);
        //                            if (question?.reponses == null)
        //                                continue;

        //                            bool hasCorrectChecked = question.reponses.Any(mapped =>
        //                            mapped.IsAnswer && q.Answers.Any(a => a.Body == mapped.Body && a.IsChecked));
        //                            //if (!hasCorrectChecked)
        //                            //    continue;

        //                            double score = 0;
        //                            x.RelativeItem().
        //                            Padding(10).
        //                            Text(question.questionText);
        //                            foreach (var mapped in question.reponses)
        //                            {
        //                                double coefficient = (double)question.note / question.propositions.Count;
        //                                bool isChecked = q.Answers.Any(a => a.Body == mapped.Body && a.IsChecked);
        //                                bool isMatch = mapped.IsAnswer == isChecked;

        //                                if (hasCorrectChecked)
        //                                    score += isMatch ? coefficient : -coefficient;
        //                                //else if (isChecked && !mapped.IsAnswer)
        //                                //    score -= coefficient;

        //                            }
        //                                x.RelativeItem().
        //                                        Padding(10).
        //                                        Text(score);
        //                            total += score;
        //                        }

        //                    });




        //                page.Footer()
        //                    .AlignCenter()
        //                    .Text(x =>
        //                    {
        //                        x.Span("Page ");
        //                        x.CurrentPageNumber();
        //                    });
        //            });
        //        })
        //           .GeneratePdf();
        //        return await Task.FromResult(pdfBytes);
        //    }
        //    catch (Exception ex) 
        //    {
        //        return  new byte[0] ; 
        //    }
        //}
   

public async Task<byte[]> GenerateQuizReportAsync(QuizReportRequestDTO request)
        {
            try
            {
                var data = await BuildReportDataAsync(request);

                var quiz = data.Quiz;
                var reportQuestions = data.Questions;
                double total = data.Total;
                double maxTotal = data.MaxTotal;
                double percentage = data.Percentage;
                string mention = data.Mention;
                string mentionColor = data.MentionColor;

                // ============================================================
                // Génération du PDF
                // ============================================================

                byte[] pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);

                        page.Margin(2, Unit.Centimetre);

                        page.PageColor(Colors.White);

                        page.DefaultTextStyle(x =>
                            x.FontSize(12));


                        // ====================================================
                        // HEADER
                        // ====================================================

                        page.Header()
                            .Background(Colors.Blue.Darken2)
                            .Padding(16)
                            .Row(headerRow =>
                            {
                                headerRow.RelativeItem().Column(col =>
                                {
                                    col.Item()
                                        .Text(quiz.titre ?? "Quiz")
                                        .FontSize(22)
                                        .Bold()
                                        .FontColor(Colors.White);

                                    col.Item()
                                        .PaddingTop(4)
                                        .Text(text =>
                                        {
                                            text.Span($"Durée : {quiz.duree_quiz} min   ").FontSize(10).FontColor(Colors.Blue.Lighten4);
                                            text.Span($"Questions : {quiz.nbr_questions}").FontSize(10).FontColor(Colors.Blue.Lighten4);
                                        });
                                });

                                headerRow.ConstantItem(150).Column(col =>
                                {
                                    col.Item().AlignRight()
                                        .Text("Rapport généré le")
                                        .FontSize(9)
                                        .FontColor(Colors.Blue.Lighten4);

                                    col.Item().AlignRight()
                                        .Text(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                                        .FontSize(10)
                                        .SemiBold()
                                        .FontColor(Colors.White);
                                });
                            });


                        // ====================================================
                        // CONTENT
                        // ====================================================

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(14);

                                // ------------------------------------------------
                                // Carte résumé
                                // ------------------------------------------------

                                column.Item()
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .CornerRadius(8)
                                    .Background(Colors.Grey.Lighten5)
                                    .Padding(16)
                                    .Row(summaryRow =>
                                    {
                                        summaryRow.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text("Score total").FontSize(10).FontColor(Colors.Grey.Darken1);
                                            col.Item().Text($"{total:0.##} / {maxTotal:0.##}").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                                        });

                                        summaryRow.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text("Pourcentage").FontSize(10).FontColor(Colors.Grey.Darken1);
                                            col.Item().Text($"{percentage:0.#}%").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                                        });

                                        summaryRow.RelativeItem().Column(col =>
                                        {
                                            col.Item().AlignRight().Text("Mention").FontSize(10).FontColor(Colors.Grey.Darken1);
                                            col.Item().AlignRight()
                                                .Background(mentionColor)
                                                .PaddingVertical(4)
                                                .PaddingHorizontal(10)
                                                .Text(mention)
                                                .FontSize(12)
                                                .Bold()
                                                .FontColor(Colors.White);
                                        });
                                    });

                                // ------------------------------------------------
                                // Détail des réponses
                                // ------------------------------------------------

                                column.Item()
                                    .PaddingTop(6)
                                    .Text("Détail des réponses")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken2);

                                int index = 1;

                                foreach (var item in reportQuestions)
                                {
                                    bool isFullScore = item.Score >= item.MaxScore;
                                    string scoreColor = isFullScore
                                        ? Colors.Green.Darken1
                                        : (item.Score > 0 ? Colors.Amber.Darken2 : Colors.Red.Darken1);

                                    int questionIndex = index;

                                    column.Item()
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Background(Colors.Grey.Lighten5)
                                        .CornerRadius(6)
                                        .Padding(12)
                                        .Column(card =>
                                        {
                                            card.Spacing(6);

                                            card.Item().Row(questionRow =>
                                            {
                                                questionRow.ConstantItem(24)
                                                    .Height(24)
                                                    .Background(Colors.Blue.Darken2)
                                                    .AlignCenter()
                                                    .AlignMiddle()
                                                    .Text(questionIndex.ToString())
                                                    .FontSize(11)
                                                    .Bold()
                                                    .FontColor(Colors.White);

                                                questionRow.RelativeItem()
                                                    .PaddingLeft(8)
                                                    .AlignMiddle()
                                                    .Text(item.QuestionText)
                                                    .FontSize(13)
                                                    .SemiBold();

                                                questionRow.ConstantItem(70)
                                                    .AlignRight()
                                                    .Background(scoreColor)
                                                    .PaddingVertical(3)
                                                    .PaddingHorizontal(8)
                                                    .AlignMiddle()
                                                    .Text($"{item.Score:0.##}/{item.MaxScore:0.##}")
                                                    .FontSize(10)
                                                    .Bold()
                                                    .FontColor(Colors.White);
                                            });

                                            foreach (var prop in item.Propositions)
                                            {
                                                string indicatorColor = prop.IsCorrect
                                                    ? Colors.Green.Medium
                                                    : (prop.IsSelected ? Colors.Red.Medium : Colors.Grey.Lighten1);

                                                card.Item()
                                                    .PaddingLeft(32)
                                                    .Row(propRow =>
                                                    {
                                                        propRow.ConstantItem(10)
                                                            .Height(10)
                                                            .Background(indicatorColor)
                                                            .CornerRadius(2);

                                                        propRow.RelativeItem()
                                                            .PaddingLeft(8)
                                                            .Text(text =>
                                                            {
                                                                if (prop.IsCorrect && prop.IsSelected)
                                                                {
                                                                    text.Span(prop.Text).FontSize(11).SemiBold().FontColor(Colors.Green.Darken2);
                                                                }
                                                                else if (prop.IsCorrect && !prop.IsSelected)
                                                                {
                                                                    text.Span(prop.Text).FontSize(11).Italic().FontColor(Colors.Green.Darken1);
                                                                    text.Span("  (réponse attendue)").FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                                                                }
                                                                else if (!prop.IsCorrect && prop.IsSelected)
                                                                {
                                                                    text.Span(prop.Text).FontSize(11).FontColor(Colors.Red.Darken2);
                                                                }
                                                                else
                                                                {
                                                                    text.Span(prop.Text).FontSize(11).FontColor(Colors.Grey.Darken1);
                                                                }
                                                            });
                                                    });
                                            }

                                            if (item.IsCodeQuestion)
                                            {
                                                bool isCorrect = item.Score >= item.MaxScore;
                                                string codeColor = isCorrect ? Colors.Green.Darken1 : Colors.Red.Darken1;

                                                card.Item()
                                                    .PaddingLeft(32)
                                                    .Background(Colors.Grey.Lighten3)
                                                    .Padding(6)
                                                    .Column(codeCol =>
                                                    {
                                                        codeCol.Spacing(3);

                                                        codeCol.Item().Text("Code soumis :").FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
                                                        codeCol.Item().Text(item.SubmittedCode ?? "").FontSize(9).FontFamily("Consolas");

                                                        codeCol.Item().PaddingTop(4).Text(text =>
                                                        {
                                                            text.Span("Sortie obtenue : ").FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
                                                            text.Span(item.ActualOutput ?? "").FontSize(9).FontColor(codeColor);
                                                        });

                                                        if (!isCorrect)
                                                        {
                                                            codeCol.Item().Text(text =>
                                                            {
                                                                text.Span("Sortie attendue : ").FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
                                                                text.Span(item.ExpectedOutput ?? "").FontSize(9).FontColor(Colors.Green.Darken1);
                                                            });
                                                        }
                                                    });
                                            }
                                        });

                                    index++;
                                }
                            });


                        // ====================================================
                        // FOOTER
                        // ====================================================

                        page.Footer()
                            .BorderTop(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .PaddingTop(6)
                            .Row(footerRow =>
                            {
                                footerRow.RelativeItem()
                                    .Text("QuizApp")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Medium);

                                footerRow.RelativeItem()
                                    .AlignRight()
                                    .Text(text =>
                                    {
                                        text.Span("Page ").FontSize(9).FontColor(Colors.Grey.Darken1);
                                        text.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Darken1);
                                        text.Span(" / ").FontSize(9).FontColor(Colors.Grey.Darken1);
                                        text.TotalPages().FontSize(9).FontColor(Colors.Grey.Darken1);
                                    });
                            });
                    });
                })
                .GeneratePdf();


                return pdfBytes;
            }
            catch (Exception ex)
            {
                // IMPORTANT :
                // Ne pas retourner new byte[0], sinon tu masques
                // l'erreur et le frontend reçoit un PDF vide.

                throw new Exception(
                    "Erreur lors de la génération du rapport PDF.",
                    ex);
            }
        }

        public async Task<QuizScoreSummaryDTO> GetQuizScoreSummaryAsync(QuizReportRequestDTO request)
        {
            var data = await BuildReportDataAsync(request);

            return new QuizScoreSummaryDTO
            {
                Score = data.Total,
                MaxScore = data.MaxTotal,
                Percentage = data.Percentage,
                Mention = data.Mention
            };
        }

        private async Task<QuizReportData> BuildReportDataAsync(QuizReportRequestDTO request)
        {
            var quiz = await _quizRepository.GetQuizById(request.QuizId);

            if (quiz == null)
                throw new Exception("Quiz introuvable.");

            // On prépare toutes les données AVANT QuestPDF
            var reportQuestions = new List<QuizQuestionReportDTO>();

            double total = 0;

            foreach (var q in request.ListScoreRequest)
            {
                var question = await _questionRepository
                    .GetQuestionById(q.QuestionId);

                if (question == null)
                    continue;

                if (question.reponses == null || !question.reponses.Any())
                    continue;

                if (question.type == "Codage")
                {
                    try
                    {
                        var reponse = question.reponses.FirstOrDefault();
                        var actualOutput = q.SubmittedOutput ?? string.Empty;
                        bool isCorrect = string.Equals(actualOutput.Trim(), reponse?.output?.Trim() ?? string.Empty, StringComparison.Ordinal);
                        double codeScore = isCorrect ? question.note : 0;

                        total += codeScore;

                        reportQuestions.Add(new QuizQuestionReportDTO
                        {
                            QuestionText = question.questionText ?? "",
                            Score = codeScore,
                            MaxScore = question.note,
                            IsCodeQuestion = true,
                            SubmittedCode = q.SubmittedCode,
                            ExpectedOutput = reponse?.output,
                            ActualOutput = actualOutput
                        });
                    }
                    catch
                    {
                        // Une question Code défaillante ne doit pas casser le score des autres questions.
                        reportQuestions.Add(new QuizQuestionReportDTO
                        {
                            QuestionText = question.questionText ?? "",
                            Score = 0,
                            MaxScore = question.note,
                            IsCodeQuestion = true,
                            SubmittedCode = q.SubmittedCode
                        });
                    }

                    continue;
                }

                if (question.propositions == null || !question.propositions.Any())
                    continue;

                bool hasCorrectChecked = question.reponses.Any(mapped =>
                    mapped.IsAnswer &&
                    q.Answers != null &&
                    q.Answers.Any(a =>
                        a.Body == mapped.Body &&
                        a.IsChecked));

                double score = 0;

                double coefficient =
                    (double)question.note / question.propositions.Count;

                foreach (var mapped in question.reponses)
                {
                    bool isChecked =
                        q.Answers != null &&
                        q.Answers.Any(a =>
                            a.Body == mapped.Body &&
                            a.IsChecked);

                    bool isMatch = mapped.IsAnswer == isChecked;

                    if (hasCorrectChecked)
                    {
                        score += isMatch
                            ? coefficient
                            : 0;
                    }
                }

                total += score;

                var propositionRows = question.propositions
                    .Select(p =>
                    {
                        bool isCorrect = question.reponses.Any(r =>
                            r.Body == p.textProposition && r.IsAnswer);

                        bool isSelected =
                            q.Answers != null &&
                            q.Answers.Any(a =>
                                a.Body == p.textProposition && a.IsChecked);

                        return new PropositionReportDTO
                        {
                            Text = p.textProposition ?? "",
                            IsCorrect = isCorrect,
                            IsSelected = isSelected
                        };
                    })
                    .ToList();

                reportQuestions.Add(new QuizQuestionReportDTO
                {
                    QuestionText = question.questionText ?? "",
                    Score = score,
                    MaxScore = question.note,
                    Propositions = propositionRows
                });
            }

            double maxTotal = reportQuestions.Sum(r => r.MaxScore);
            double percentage = maxTotal > 0 ? total / maxTotal * 100 : 0;

            string mention;
            string mentionColor;
            if (percentage >= 80)
            {
                mention = "Excellent";
                mentionColor = Colors.Amber.Darken2;
            }
            else if (percentage >= 50)
            {
                mention = "Réussi";
                mentionColor = Colors.Green.Darken1;
            }
            else
            {
                mention = "Échoué";
                mentionColor = Colors.Red.Darken1;
            }

            return new QuizReportData
            {
                Quiz = quiz,
                Questions = reportQuestions,
                Total = total,
                MaxTotal = maxTotal,
                Percentage = percentage,
                Mention = mention,
                MentionColor = mentionColor
            };
        }

        private class QuizReportData
        {
            public QuizApp.Entities.Conception_Entities.Quiz Quiz { get; set; }

            public List<QuizQuestionReportDTO> Questions { get; set; }

            public double Total { get; set; }

            public double MaxTotal { get; set; }

            public double Percentage { get; set; }

            public string Mention { get; set; }

            public string MentionColor { get; set; }
        }

        public class QuizQuestionReportDTO
        {
            public string QuestionText { get; set; } = string.Empty;

            public double Score { get; set; }

            public double MaxScore { get; set; }

            public List<PropositionReportDTO> Propositions { get; set; } = new();

            public bool IsCodeQuestion { get; set; }

            public string? SubmittedCode { get; set; }

            public string? ExpectedOutput { get; set; }

            public string? ActualOutput { get; set; }
        }

        public class PropositionReportDTO
        {
            public string Text { get; set; } = string.Empty;

            public bool IsCorrect { get; set; }

            public bool IsSelected { get; set; }
        }

    }
}
