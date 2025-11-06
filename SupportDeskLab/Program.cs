using System;
using System.Collections.Generic;
using static SupportDeskLab.Utility;


namespace SupportDeskLab
{
   
     
    class Program
    {
        static int NextTicketId = 1;

        //Create Customer Dictionary
        static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();
        //create Ticket Queue
        static Queue<Ticket> TicketQueue = new Queue<Ticket>();
        //Create UndoEvent stack
        static Stack<UndoEvent> UndoStack = new Stack<UndoEvent>();
        static void Main()
        {
            initCustomer();

            while (true)
            {
                Console.WriteLine("\n=== Support Desk ===");
                Console.WriteLine("[1] Add customer");
                Console.WriteLine("[2] Find customer");
                Console.WriteLine("[3] Create ticket");
                Console.WriteLine("[4] Serve next ticket");
                Console.WriteLine("[5] List customers");
                Console.WriteLine("[6] List tickets");
                Console.WriteLine("[7] Undo last action");
                Console.WriteLine("[0] Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                //create switch cases and then call a reletive method 
                //for example for case 1 you need to have a method named addCustomer(); or case 2 add a method name findCustomer

                switch (choice)
                {
                    case "1": AddCustomer(); break;
                    case "2": FindCustomer(); break;
                    case "3": CreateTicket(); break;
                    case "4": ServeNext(); break;
                    case "5": ListCustomers(); break;
                    case "6": ListTickets(); break;
                    case "7": Undo(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }
        /*
         * Do not touch initCustomer method. this is like a seed to have default customers.
         */
        static void initCustomer()
        {
            //uncomments these 3 lines after you create the Customer Dictionary
            Customers["C001"] = new Customer("C001", "Ava Martin", "ava@example.com");
            Customers["C002"] = new Customer("C002", "Ben Parker", "ben@example.com");
            Customers["C003"] = new Customer("C003", "Chloe Diaz", "chloe@example.com");
        }

        static void AddCustomer()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("New CustomerId (e.g., C09d0): ");
            string customerId = Console.ReadLine();

            if (Customers.ContainsKey(customerId))
            {
                Console.WriteLine("Customer ID already exists.");
                return;
            }

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Customer newCustomer = new Customer(customerId, name, email);
            Customers[customerId] = newCustomer;
            UndoStack.Push(new UndoAddCustomer(newCustomer));

            Console.WriteLine($"Added: {newCustomer}");

        }

        static void FindCustomer()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("Enter CustomerId: ");
            string customerId = Console.ReadLine();

            if (Customers.TryGetValue(customerId, out Customer customer))
            {
                Console.WriteLine($"Found: {customer}");
            }
            else
            {
                Console.WriteLine("Not found.");
            }

        }

        static void CreateTicket()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("CustomerId: ");
            string customerId = Console.ReadLine();

            if (!Customers.ContainsKey(customerId))
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.Write("Subject: ");
            string subject = Console.ReadLine();

            Ticket newTicket = new Ticket(NextTicketId++, customerId, subject);
            TicketQueue.Enqueue(newTicket);
            UndoStack.Push(new UndoCreateTicket(newTicket));

            Console.WriteLine($"Created: {newTicket}");
        }

        static void ServeNext()
        {
            //look at the Demo captuerd image and add your code here
            if (TicketQueue.Count == 0)
            {
                Console.WriteLine("No tickets in queue.");
                return;
            }

            Ticket servedTicket = TicketQueue.Dequeue();
            UndoStack.Push(new UndoServeTicket(servedTicket));

            Console.WriteLine($"Served ticket: {servedTicket}");
        }

        static void ListCustomers()
        {
            Console.WriteLine("-- Customers --");
            //look at the Demo captuerd image and add your code here
            foreach (var customer in Customers.Values)
            {
                Console.WriteLine(customer);
            }
        }

        static void ListTickets()
        {
           
            Console.WriteLine("-- Tickets (front to back) --");
            //look at the Demo captuerd image and add your code here
            foreach (var ticket in TicketQueue)
            {
                Console.WriteLine(ticket);
            }
        }

        static void Undo()
        {
            //look at the Demo captuerd image and add your code here
            if (UndoStack.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            UndoEvent lastEvent = UndoStack.Pop();

            switch (lastEvent)
            {
                case UndoAddCustomer undoAdd:
                    Customers.Remove(undoAdd.Customer.CustomerId);
                    Console.WriteLine($"Undo: Removed customer {undoAdd.Customer.CustomerId}");
                    break;

                case UndoCreateTicket undoTicket:
                    var tempQueue = new Queue<Ticket>();
                    bool found = false;

                    while (TicketQueue.Count > 0)
                    {
                        var ticket = TicketQueue.Dequeue();
                        if (ticket.TicketId != undoTicket.Ticket.TicketId)
                        {
                            tempQueue.Enqueue(ticket);
                        }
                        else
                        {
                            found = true;
                        }
                    }

                    TicketQueue = tempQueue;
                    if (found)
                    {
                        Console.WriteLine($"Undo: Deleted ticket #{undoTicket.Ticket.TicketId}");
                    }
                    break;

                case UndoServeTicket undoServe:
                    TicketQueue.Enqueue(undoServe.Ticket);
                    Console.WriteLine($"Undo: Restored ticket #{undoServe.Ticket.TicketId}");
                    break;
            }
        }
    }
}

