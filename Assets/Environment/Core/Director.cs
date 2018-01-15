using UnityEngine;
using System.Collections.Generic;
using System;

namespace RobotNursery.Environment
{
    [RequireComponent(typeof(Supervisee), typeof(History))]
    public partial class Director : MonoBehaviour
    {
        private List<Scenario.Session> sessions
            = new List<Scenario.Session>();

        private Supervisee supervisee;
        private History history;

        private void Awake()
        {
            supervisee = GetComponent<Supervisee>();
            history = GetComponent<History>();
        }

        public void Step()
        {
            sessions.ForEach(session => session.Step());
        }

        protected void Begin(Scenario scenario)
        {
            Scenario.Session session = FindSession(scenario);

            if (session == null)
            {
                session = scenario.CreateSession(supervisee);

                sessions.Add(session);

                session.EndEvent += OnSessionEnd;
                session.Begin();

                history.BeginTrial(scenario);
            }
        }

        private void OnSessionEnd(object obj, EventArgs args)
        {
            var session = obj as Scenario.Session;

            End(session.Parent);
        }

        protected void End(Scenario scenario)
        {
            Scenario.Session session = FindSession(scenario);

            if (session != null)
            {
                End(session);
            }
        }

        private void End(Scenario.Session session)
        {
            session.EndEvent -= OnSessionEnd;
            session.End();

            sessions.Remove(session);

            history.EndTrial(session.Parent);
        }

        protected T GetScenario<T>() where T : Scenario
        {
            var scenarios = GetScenarios();

            return scenarios.GetComponent<T>() ?? scenarios.AddComponent<T>();
        }

        private GameObject GetScenarios()
        {
            // TODO: proper container(s) for scenarios
            var scenarios = GameObject.Find("Scenarios");

            if (scenarios == null)
            {
                throw new InvalidOperationException("No scenarios available");
            }

            return scenarios;
        }

        private Scenario.Session FindSession(Scenario scenario)
        {
            return sessions.Find(s => s.Parent == scenario);
        }
    }
}
