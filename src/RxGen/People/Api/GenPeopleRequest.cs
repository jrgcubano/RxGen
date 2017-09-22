using System.Collections.Generic;
using RxGen.Core.Extensions;
using RxGen.People.Models;

namespace RxGen.People.Api
{
    public class GenPeopleRequest
    {
        private HashSet<Nationality> _nationalities;

        private HashSet<Field> _includeFields;

        private HashSet<Field> _excludeFields;

        public GenPeopleRequest()
        {
            _nationalities = new HashSet<Nationality>();
            _includeFields = new HashSet<Field>();
            _excludeFields = new HashSet<Field>();
            Page = 0;
            Results = 0;
            Gender = Gender.Both;
        }

        public Gender Gender { get; private set; }

        public string Seed { get; private set; }

        public int Page { get; private set; }

        public int Results { get; private set; }

        public IEnumerable<Nationality> Nationalities => _nationalities.ToHashSet();

        public IEnumerable<Field> IncludeFields => _includeFields.ToHashSet();

        public IEnumerable<Field> ExcludeFields => _excludeFields.ToHashSet();

        public void SetGender(Gender gender)
        {
            Gender = gender;
        }

        public void SetPage(int page)
        {
            PeopleRequestGuard.ValidPage(page, nameof(page));

            Page = page;
        }

        public void SetResults(int results)
        {
            PeopleRequestGuard.ValidResults(results, nameof(results));

            Results = results;
        }

        public void AddNationality(Nationality nationality)
        {
            _nationalities.Add(nationality);
        }

        public void SetSeed(string seed)
        {
            Seed = seed;
        }

        public void IncludeField(Field field)
        {
            PeopleRequestGuard.NotFields(_excludeFields, nameof(_excludeFields));

            _includeFields.Add(field);
        }

        public void ExcludeField(Field field)
        {
            PeopleRequestGuard.NotFields(_includeFields, nameof(_includeFields));

            _excludeFields.Add(field);
        }

        public string AsUrl()
        {
            var urlParams = new List<string>();
            if (Page > 0)
                urlParams.Add($"page={Page}");
            if (Results > 0)
                urlParams.Add($"results={Results}");
            if (!string.IsNullOrEmpty(Seed))
                urlParams.Add($"seed={Seed}");
            if (Gender != Gender.Both)
                urlParams.Add($"gender={Gender.ToString().ToLower()}");
            if (_includeFields.Count > 0)
                urlParams.Add($"inc={string.Join(",", _includeFields)}");
            if (_excludeFields.Count > 0)
                urlParams.Add($"exc={string.Join(",", _excludeFields)}");
            if (_nationalities.Count > 0)
                urlParams.Add($"nat={string.Join(",", _nationalities)}");
            var url = urlParams.Count > 0
                ? $"?{string.Join("&", urlParams)}"
                : string.Empty;
            return url;
        }
    }
}