using tracker.Enums;
using tracker.Models;
using tracker.Interfaces;

using System.Text.Json;

namespace tracker.Services {
    public class NinmuServices : INinmuServices {
        private static string fileName = "ninmuData.json";
        private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

        public Task<int> addNinmu(string m) {
            try {
                var risuto = new List<Ninmu>();
                var newOne = new Ninmu {
                    ID = getNinmuID(),
                    description = m,
                    ninmuJyoutai = Status.todo,
                    createdAt = DateTime.UtcNow,
                    updatedAt = DateTime.UtcNow
                };

                bool dekitaka = createFileIfNotExist();

                if (dekitaka) {
                    risuto = getRisutoFromJson();
                    risuto?.Add(newOne);
                    updateJsonFile(risuto?? []);
                    return Task.FromResult(newOne.ID);
                }

                return Task.FromResult(0);

            } catch (Exception e) {
                Console.WriteLine("Failed at adding new Ninmu. Error - " + e.Message);
                return Task.FromResult(0);
            }
        }

        public Task<bool> updateNinmu(int i, string m) {
            if (!File.Exists(filePath))
                return Task.FromResult(false);

            var risuto = getRisutoFromJson();

            if (risuto.Count > 0) {
                var oldOne = risuto.Where(n => n.ID == i).SingleOrDefault();
                if (oldOne != null) {
                    var newOne = new Ninmu {
                        ID = i,
                        description = m,
                        ninmuJyoutai = oldOne.ninmuJyoutai,
                        createdAt = oldOne.createdAt,
                        updatedAt = DateTime.UtcNow
                    };
                    risuto.Remove(oldOne);
                    risuto.Add(newOne);
                    updateJsonFile(risuto);
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        public Task<bool> deleteNinmu(int i) {
            if (!File.Exists(filePath))
                return Task.FromResult(false);

            var risuto = getRisutoFromJson();

            if (risuto.Count > 0) {
                var deletingOne = risuto.Where(n => n.ID == i).SingleOrDefault();
                if (deletingOne != null) {
                    risuto.Remove(deletingOne);
                    updateJsonFile(risuto);
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        public Task<bool> statusSetti(int i, string m) {
            if (!File.Exists(filePath))
                return Task.FromResult(false);
            
            var risuto = getRisutoFromJson();

            if (risuto.Count > 0) {
                var oldOne = risuto.Where(n => n.ID == i).SingleOrDefault();
                if (oldOne != null) {
                    var newOne = new Ninmu {
                        ID = i,
                        description = oldOne.description,
                        ninmuJyoutai = SettiStringToStatus(m),
                        createdAt = oldOne.createdAt,
                        updatedAt = DateTime.UtcNow
                    };
                    risuto.Remove(oldOne);
                    risuto.Add(newOne);
                    updateJsonFile(risuto);
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }

        public Task<List<Ninmu>> getAllNinmu() {
            try {
                return Task.FromResult(getRisutoFromJson());

            } catch (Exception e) {
                Console.WriteLine("Failed at fetching all ninmus. Error - " + e.Message);
                return Task.FromResult(new List<Ninmu>());
            }
        }

        public Task<List<Ninmu>> getNinmuByStatus(string m) {
            try {
                if (!File.Exists(filePath))
                    return Task.FromResult(new List<Ninmu>());

                var risuto = getRisutoFromJson();

                if (risuto.Count > 0) 
                    return Task.FromResult(risuto.Where(n => n.ninmuJyoutai == GetStringToStatus(m)).ToList());
                else
                    return Task.FromResult(new List<Ninmu>());

            } catch (Exception e) {
                Console.WriteLine("Failed at fetching specific ninmu(s)" + e.Message);
                return Task.FromResult(new List<Ninmu>());
            }        
        }

        public List<string> getAllCommands() {
            return new List<string> {
                @"add \Ninmu Description\ - To add a new ninmu, type add with ninmu description",
                @"update \Ninmu ID\ \Ninmu Description\ - To update a ninmu, type update with ninmu ID and ninmu description",
                @"delete \Ninmu ID\ - To delete a ninmu, type delete with ninmu ID",
                @"mark-progressing \Ninmu ID\ - To mark a ninmu to progressing, type mark-progressing with ninmu ID",
                @"mark-done \Ninmu ID\ - To mark a ninmu to done, type mark-done with ninmu ID",
                "list - To list all ninmu with current status",
                "list todo  - To list all ninmus with todo status",
                "list progressing  - To list all ninmu with progressing status",
                "list done - To list all ninmus with done status",
                "exit - To exit from app",
                "clear - To clear console window"
            };
        }

        #region helper methods
        private int getNinmuID() {
            if (!File.Exists(filePath))
                return 1;
            else {
                string currentJson = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(currentJson)) {
                    var risuto = JsonSerializer.Deserialize<List<Ninmu>>(currentJson);
                    if (risuto != null && risuto.Count > 0)
                        return risuto.OrderBy(n => n.ID).Last().ID + 1;
                }
            }

            return 1;
        }

        private bool createFileIfNotExist() {
            try {
                if (!File.Exists(filePath)) {}
                    using (FileStream i = File.Create(filePath))
                        Console.WriteLine($"Succeeded in creating {fileName}.");
                
                return true;

            } catch (Exception e) {
                Console.WriteLine($"Failed at creating {fileName}. Error - " + e.Message);
                return false;
            }
        }

        private List<Ninmu> getRisutoFromJson() {
            if (!File.Exists(filePath))
                return new List<Ninmu>();

            string currentJson = File.ReadAllText(filePath);

            if (!string.IsNullOrEmpty(currentJson))
                return JsonSerializer.Deserialize<List<Ninmu>>(currentJson)?? [];

            return new List<Ninmu>();
        }

        private void updateJsonFile(List<Ninmu> i) {
            string updatedJson = JsonSerializer.Serialize<List<Ninmu>>(i);
            File.WriteAllText(filePath, updatedJson);
        }

        private Status SettiStringToStatus(string m) {
            switch (m) {
                case "mark-todo":
                    return Status.todo;
                case "mark-progressing":
                    return Status.progressing;
                case "mark-done":
                    return Status.done;
                default:
                    return Status.todo;
            }
        }

        private Status GetStringToStatus(string m) {
            switch (m) {
                case "todo":
                    return Status.todo;
                case "progressing":
                    return Status.progressing;
                case "done":
                    return Status.done;
                default:
                    return Status.todo;
            }
        }
        #endregion
    }
}