using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using RxGen.People.Api;
using RxGen.People.Models;

namespace RxGen.People
{
    /// <summary>
    /// Defines a people random generator builder
    /// </summary>
    public interface IRxPeople
    {
        /// <summary>
        /// Specify the ammount of users to generate per page
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns>builder instance</returns>
        IRxPeople Ammount(int ammount);

        /// <summary>
        /// Specify the gender of users to generate
        /// </summary>
        /// <param name="gender">female, male or both by default</param>
        /// <returns>builder instance</returns>
        IRxPeople Gender(Gender gender);

        /// <summary>
        /// Allows to always generate the same set of users
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>builder instance</returns>
        IRxPeople Seed(string seed);

        /// <summary>
        /// Specify the allowed nationalities to generate the users
        /// </summary>
        /// <param name="nationality">first nationality</param>
        /// <param name="nationalities">the rest of nationalities</param>
        /// <returns>builder instance</returns>
        IRxPeople Nationality(Nationality nationality, params Nationality[] nationalities);

        /// <summary>
        /// Specify the page of results to request in combination
        /// with the seed group and amount of results
        /// </summary>
        /// <param name="page">page to request</param>
        /// <returns>builder instance</returns>
        IRxPeople Page(int page);

        /// <summary>
        /// Specify the allowed nationalities to generate the users
        /// </summary>
        /// <param name="field">first person field</param>
        /// <param name="fields">the rest of fields to include</param>
        /// <returns>builder instance</returns>
        IRxPeople IncludeField(Field field, params Field[] fields);

        /// <summary>
        /// Specify the allowed nationalities to generate the users
        /// </summary>
        /// <param name="field">first person field</param>
        /// <param name="fields">the rest of fields to include</param>
        /// <returns>builder instance</returns>
        IRxPeople ExcludeField(Field field, params Field[] fields);

        /// <summary>
        /// Returns request object
        /// </summary>
        GenPeopleRequest AsRequest();

        /// <summary>
        /// Return request call as a Task
        /// </summary>
        Task<GenPeopleResponse> AsTask();

        /// <summary>
        /// Return request call as an Observable
        /// </summary>
        IObservable<GenPeopleResponse> AsObservable();

        /// <summary>
        /// Return request call as an Observable
        /// </summary>
        /// <param name="scheduler">scheduler to use</param>
        IObservable<GenPeopleResponse> AsObservable(IScheduler scheduler);
    }
}