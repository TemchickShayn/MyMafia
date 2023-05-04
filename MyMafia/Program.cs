using System;
using System.Collections.ObjectModel;
namespace MyMafia
{
    public class Player
    {
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public Role Role { get; set; }

        public Player(string name, Role role)
        {
            Name = name;
            Role = role;
            IsAlive = true;
        }
    }
    public enum Role
    {
        Mafia,
        Doctor,
        Detective,
        Civilian
    }
    public class Mafia : Player
    {
        public Mafia(string name) : base(name, Role.Mafia) { }
        public void CommitMurder(Player player)
        {
            if (player.Role != Role.Mafia && player.IsAlive)
            {
                player.IsAlive = false;
            }
        }

    }
    public class Doctor : Player
    {
        public Doctor(string name) : base(name, Role.Doctor) { }

        public void Heal(Player player)
        {
            player.IsAlive = true;
        }
    }

    public class Detective : Player
    {
        public Detective(string name) : base(name, Role.Detective) { }

        public bool Investigate(Player player)
        {
            return player.Role == Role.Mafia;
        }
    }

    public class Civilian : Player
    {
        public Civilian(string name) : base(name, Role.Civilian) { }
    }

    public class MafiaGame
    {
        private List<Player> players;
        private List<Player> alivePlayers;
        private List<Mafia> mafiaPlayers;
        private Doctor doctorPlayer;
        private Detective detectivePlayer;
        public bool isGameOver;

        public object Arrays { get; private set; }

        public MafiaGame(List<Player> players, Mafia mafia1, Mafia mafia2, Doctor doctor, Detective detective)
        {
            this.players = players;
            alivePlayers = new List<Player>(players);
            mafiaPlayers = new List<Mafia> { mafia1, mafia2 };
            doctorPlayer = doctor;
            detectivePlayer = detective;
        }
        public static List<Player> AssignRoles(List<string> playerNames)
        {
            List<Player> players = new List<Player>();
            int mafiaCount = playerNames.Count / 3; 

           
            Random random = new Random();
            for (int i = 0; i < playerNames.Count; i++)
            {
                string name = playerNames[i];
                if (mafiaCount > 0 && random.Next(1, 4) == 1) 
                {
                    players.Add(new Mafia(name));
                    mafiaCount--;
                }
                else if (random.Next(1, 4) == 1) 
                {
                    int role = random.Next(1, 3);
                    if (role == 1)
                    {
                        players.Add(new Doctor(name));
                    }
                    else
                    {
                        players.Add(new Detective(name));
                    }
                }
                else 
                {
                    players.Add(new Civilian(name));
                }
            }

            return players;
        }
        public void Day()
        {
            
            if (isGameOver)
            {
                return;
            }
          
            Dictionary<Player, int> votes = new Dictionary<Player, int>();
            int numVotes = 0;
            int numPlayers = alivePlayers.Count;

            while (numVotes < numPlayers)
            {
                Player voter = alivePlayers[numVotes % numPlayers];
                Console.WriteLine(voter.Name + ", who do you vote to eliminate?");
                string voteName = Console.ReadLine();
                try
                {
                    int.Parse(voteName); 
                    throw new Exception("Entered a number or symbol instead of the player's name!");
                }
                catch (FormatException)
                {
                    Console.WriteLine("The number is not entered, it's okay.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Player votePlayer = alivePlayers.Find(p => p.Name == voteName);
                if (votePlayer != null && votePlayer.IsAlive)
                {
                    if (votes.ContainsKey(votePlayer))
                    {
                        votes[votePlayer]++;
                    }
                    else
                    {
                        votes.Add(votePlayer, 1);
                    }
                }
                numVotes++;
            }
           
            Player eliminatedPlayer = null;
            int maxVotes = 0;
            foreach (KeyValuePair<Player, int> kvp in votes)
            {
                if (kvp.Value > maxVotes)
                {
                    eliminatedPlayer = kvp.Key;
                    maxVotes = kvp.Value;
                }
            }
         
            Console.WriteLine("Player " + eliminatedPlayer.Name + " has been eliminated.");
            eliminatedPlayer.IsAlive = false;
            alivePlayers.Remove(eliminatedPlayer);
           
            CheckGameOver();
        }


            public void CheckGameOver()
            {
               
                bool mafiaAlive = false;
                bool civiliansAlive = false;
                foreach (Player player in alivePlayers)
                {
                    if (player.Role == Role.Mafia)
                    {
                        mafiaAlive = true;
                    }
                    else if (player.Role != Role.Mafia)
                    {
                        civiliansAlive = true;
                    }
                }
                
                if (!mafiaAlive)
                {
                    Console.WriteLine("Civilian players have won!");
                    isGameOver = true;
                }
                
                else if (!civiliansAlive)
                {
                    Console.WriteLine("Mafia players have won!");
                    isGameOver = true;
                }
                
                else
                {
                    Console.WriteLine("No winner yet. The game continues...");
                }
            }
        public void Night()
        {
            
            if (isGameOver)
            {
                return;
            }
           
            Console.WriteLine("Mafia, who do you choose to eliminate?");
            string mafiaChoice = Console.ReadLine();
            try
            {
                int.Parse(mafiaChoice);
                throw new Exception("Entered a number or symbol instead of the player's name!");
            }
            catch (FormatException)
            {
                Console.WriteLine("The number is not entered, it's okay.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Player mafiaTarget = alivePlayers.Find(p => p.Name == mafiaChoice && p.Role != Role.Mafia);
            if (mafiaTarget != null)
            {
                foreach (Mafia mafiaPlayer in mafiaPlayers)
                {
                    mafiaPlayer.CommitMurder(mafiaTarget);
                }
                Console.WriteLine("Player " + mafiaTarget.Name + " has been eliminated by the mafia.");
                mafiaTarget.IsAlive = false;
                alivePlayers.Remove(mafiaTarget);
            }
            
            Console.WriteLine("Doctor, who do you choose to heal?");
            string doctorChoice = Console.ReadLine();
            try
            {
                int.Parse(doctorChoice);
                throw new Exception("Entered a number or symbol instead of the player's name!");
            }
            catch (FormatException)
            {
                Console.WriteLine("The number is not entered, it's okay.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Player doctorTarget = alivePlayers.Find(p => p.Name == doctorChoice && p.Role != Role.Doctor);
            if (doctorTarget != null)
            {
                doctorPlayer.Heal(doctorTarget);
                Console.WriteLine("Player " + doctorTarget.Name + " has been healed by the doctor.");
            }
            
            Console.WriteLine("Detective, who do you choose to investigate?");
            string detectiveChoice = Console.ReadLine();
            try
            {
                int.Parse(detectiveChoice);
                throw new Exception("Entered a number or symbol instead of the player's name!");
            }
            catch (FormatException)
            {
                Console.WriteLine("The number is not entered, it's okay.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Player detectiveTarget = alivePlayers.Find(p => p.Name == detectiveChoice && p.Role != Role.Detective);
            if (detectiveTarget != null)
            {
                detectivePlayer.Investigate(detectiveTarget);
                Console.WriteLine("Player " + detectiveTarget.Name + " has been investigated by the detective");
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {          
            List<string> playerNames = new List<string> { "Alice", "Bob", "Charlie", "Dave", "Eve", "Frank", "Grace", "Heidi", "Ivan" };
            List<Player> players = MafiaGame.AssignRoles(playerNames);
           
            Mafia mafia1 = new Mafia("Mafia 1");
            Mafia mafia2 = new Mafia("Mafia 2");
            Doctor doctor = new Doctor("Doctor");
            Detective detective = new Detective("Detective");
            
            MafiaGame game = new MafiaGame(players, mafia1, mafia2, doctor, detective);

            while (!game.isGameOver)
            {
                game.Night();
                game.Day();
                game.CheckGameOver();
            }
        }
    }
}