using NoSQL_project.Models;

namespace NoSQL_project.Services
{
    public class TicketSearchService
    {
        public List<Ticket> SearchTickets(List<Ticket> tickets, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return tickets.OrderByDescending(t => t.Date).ToList();

            var searchTerms = ParseSearchQuery(searchQuery);
            var results = new List<Ticket>();

            foreach (var ticket in tickets)
            {
                if (MatchesSearch(ticket, searchTerms))
                {
                    results.Add(ticket);
                }
            }

            return results.OrderByDescending(t => t.Date).ToList();
        }

        private List<SearchTerm> ParseSearchQuery(string query)
        {
            var terms = new List<SearchTerm>();
            var parts = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            string currentTerm = "";
            string currentOperator = "OR";

            foreach (var part in parts)
            {
                var upperPart = part.ToUpper();
                
                if (upperPart == "AND" || upperPart == "OR")
                {
                    if (!string.IsNullOrEmpty(currentTerm))
                    {
                        terms.Add(new SearchTerm { Word = currentTerm.Trim(), Operator = currentOperator });
                        currentTerm = "";
                    }
                    currentOperator = upperPart;
                }
                else
                {
                    if (string.IsNullOrEmpty(currentTerm))
                    {
                        currentTerm = part;
                    }
                    else
                    {
                        currentTerm += " " + part;
                    }
                }
            }

            if (!string.IsNullOrEmpty(currentTerm))
            {
                terms.Add(new SearchTerm { Word = currentTerm.Trim(), Operator = currentOperator });
            }

            if (terms.Count == 0)
            {
                terms.Add(new SearchTerm { Word = query, Operator = "OR" });
            }

            return terms;
        }

        private bool MatchesSearch(Ticket ticket, List<SearchTerm> searchTerms)
        {
            if (searchTerms.Count == 0)
                return true;

            if (searchTerms.Count == 1)
            {
                return ContainsWord(ticket, searchTerms[0].Word);
            }

            var andGroups = new List<List<bool>>();
            var currentAndGroup = new List<bool>();

            for (int i = 0; i < searchTerms.Count; i++)
            {
                var term = searchTerms[i];
                bool termMatches = ContainsWord(ticket, term.Word);

                if (i == 0)
                {
                    currentAndGroup.Add(termMatches);
                }
                else if (term.Operator == "AND")
                {
                    currentAndGroup.Add(termMatches);
                }
                else
                {
                    if (currentAndGroup.Count > 0)
                    {
                        andGroups.Add(new List<bool>(currentAndGroup));
                        currentAndGroup.Clear();
                    }
                    currentAndGroup.Add(termMatches);
                }
            }

            if (currentAndGroup.Count > 0)
            {
                andGroups.Add(currentAndGroup);
            }

            bool finalResult = false;
            foreach (var andGroup in andGroups)
            {
                bool groupResult = andGroup.All(x => x);
                finalResult = finalResult || groupResult;
            }

            return finalResult;
        }

        private bool ContainsWord(Ticket ticket, string word)
        {
            var lowerWord = word.ToLower();
            var subject = ticket.IncidentSubject?.ToLower() ?? "";
            var description = ticket.Description?.ToLower() ?? "";
            var type = ticket.IncidentType?.ToLower() ?? "";

            return subject.Contains(lowerWord) || 
                   description.Contains(lowerWord) || 
                   type.Contains(lowerWord);
        }

        private class SearchTerm
        {
            public string Word { get; set; }
            public string Operator { get; set; }
        }
    }
}

