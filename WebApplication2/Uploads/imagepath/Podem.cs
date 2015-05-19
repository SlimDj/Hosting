using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;



namespace app_v1._0
{
    public partial class Form1 : Form
    {
        #region def
        public static readonly Dictionary<EntityType, RothEnum>
            NonConrollingInputValues =
            new Dictionary<EntityType, RothEnum>
                {
                    {EntityType.And, RothEnum.One},
                    {EntityType.Nand, RothEnum.One},
                    {EntityType.Or, RothEnum.Zero},
                    {EntityType.Nor, RothEnum.Zero},
                    {EntityType.Xor, RothEnum.X},
                    {EntityType.Buff, RothEnum.X}
                };

        public static readonly Dictionary<EntityType, RothEnum>
            ControllingInputValues =
            new Dictionary<EntityType, RothEnum>
                {
                    {EntityType.And, RothEnum.Zero},
                    {EntityType.Nand, RothEnum.Zero},
                    {EntityType.Or, RothEnum.One},
                    {EntityType.Nor, RothEnum.One},
                    {EntityType.Xor, RothEnum.X},
                    {EntityType.Buff, RothEnum.X}
                };

        public static readonly Dictionary<EntityType, RothEnum>
            EasyConrolledOutputValues =
            new Dictionary<EntityType, RothEnum>
                {
                    {EntityType.And, RothEnum.Zero},
                    {EntityType.Nand, RothEnum.One},
                    {EntityType.Or, RothEnum.One},
                    {EntityType.Nor, RothEnum.Zero},
                    {EntityType.Xor, RothEnum.X},
                    {EntityType.Buff, RothEnum.X}
                };

        public static readonly Dictionary<EntityType, RothEnum>
            HardControlledOutputValues =
                new Dictionary<EntityType, RothEnum>
                    {
                        {EntityType.And, RothEnum.One},
                        {EntityType.Nand, RothEnum.Zero},
                        {EntityType.Or, RothEnum.Zero},
                        {EntityType.Nor, RothEnum.One},
                        {EntityType.Xor, RothEnum.X},
                        {EntityType.Buff, RothEnum.X}
                    };
        #endregion
        List<Elem> glob = new List<Elem>();
        List<Gate> gates = new List<Gate>();
        Graph<Gate> Graph = new Graph<Gate>();
        Graph<Gate> Graph229 = new Graph<Gate>();
        bool FaultAktivated = false;
        Queue<GraphNode<Gate>> gateQueue = new Queue<GraphNode<Gate>>();
        List<GraphNode<Gate>> Dfront = new List<GraphNode<Gate>>();
        Stack<ImplicationStackItem> implicationstack= new Stack<ImplicationStackItem>();
        NodeList<Gate> eqwert = new NodeList<Gate>();
        string pat;
        public DateTime StartTime
        {
            get;
            private set;
        }

        public DateTime EndTime
        {
            get;
            private set;
        }
        
        public Form1()
        {
            InitializeComponent();
           
        }
        
        
        private void Levelize(Graph<Gate> Graph)
        {
            textBox2.AppendText("Levelize...");
            DateTime time = DateTime.Now;
            List<Elem> inputlist = new List<Elem>();
            
            foreach (Elem elem in glob)
            {
                elem.Level = -1;
                if (elem.Type != EntityType.Input) continue;
                elem.Value = new RothValue( RothEnum.X);
                elem.Level = 0;
                inputlist.Add(elem);
            }
            foreach (GraphNode<Gate> r in Graph.Nodes)
            {
                foreach(Elem elem in inputlist)
                {
                    if (r.Value.ArIn.Contains(elem)) {gateQueue.Enqueue(r); break;}
                }
            }
            while (gateQueue.Count != 0)
            {
                GraphNode<Gate> s = new GraphNode<Gate>(); 
                s=gateQueue.Dequeue();
                queuew(s.Neighbors,0);
            }
            foreach (GraphNode<Gate> gate in Graph.Nodes) 
            {
                List<int> t = new List<int>();
                foreach (Elem w in gate.Value.ArIn)
                {
                    Elem q = glob.Find(delegate(Elem bk)
                    {
                        return bk.Name == w.Name;
                    });
                    w.Level = q.Level;
                    t.Add(w.Level);
                }
                gate.Value.Level = t.Max()+1;
                Elem q2 = glob.Find(delegate(Elem bk)
                {
                    return bk.Name == gate.Value.ArOut.Name;
                });
                q2.Level = gate.Value.Level;
            }
            DateTime time2 = DateTime.Now;
            textBox2.AppendText((time2-time).Seconds.ToString()+","+(time2-time).Milliseconds.ToString()+" сек.\n");
            
            

        }
        private void queuew(NodeList<Gate> eqwerty,int i)
        {
            i++;
            foreach (GraphNode<Gate> gate in eqwerty) 
            {
                Elem w = glob.Find(delegate(Elem bk)
                {
                    return bk.Name == gate.Value.ArOut.Name;
                });
                if (w.Level <= i) w.Level = i;
                queuew(gate.Neighbors, i);
            }
            
        }
        
        private Graph<Gate> FillTree(List<Gate> gat)
        {
            textBox2.AppendText("Creating Graph...");
            DateTime time = DateTime.Now;
            Graph<Gate> web = new Graph<Gate>();
            foreach (Gate gate1 in gat)
            {
                GraphNode<Gate> dobal=new GraphNode<Gate>(gate1);

                web.AddNode(dobal);
            }
            foreach (GraphNode<Gate> ga1 in web.Nodes)
            {
                foreach (GraphNode<Gate> ga2 in web.Nodes)
                {
                    foreach (Elem elem1 in ga2.Value.ArIn)
                    {
                        if (ga1.Value.ArOut.Name != elem1.Name) continue;
                        web.AddDirectedEdge(ga1, ga2);
                    }
                }
            } DateTime time2 = DateTime.Now;
            textBox2.AppendText((time2 - time).Seconds.ToString() + "," + (time2 - time).Milliseconds.ToString() + " сек.\n");
            return web;
        }

        private List<Gate> associate(List<Elem> glob1, List<Gate> gates1)
        {
            foreach (Gate s in gates1) 
            {
                s.ArOut.Value = RothEnum.X;
                List<Elem> wer = s.ArIn;
                foreach (Elem w in wer) 
                {
                    foreach (Elem q in glob1)
                    {
                        if (q.Name != w.Name) continue;
                        w.Type = q.Type;
                        w.CC0=q.CC0;
                        w.CC1=q.CC1;
                        w.Value = RothEnum.X;
                    }
                }
                s.ArOut.Type = glob1.First(t=>t.Name==s.ArOut.Name).Type;
                
            }
            return gates1;
        }
       
        //BTN Start podem
        private void button2_Click(object sender, EventArgs e)
        {
           
            Queue<GraphNode<Gate>> queue = new Queue<GraphNode<Gate>>();
            queue = CreateFaultQueue(Graph);
            StartTime = DateTime.Now;
            while (queue.Count != 0)
            {
                GraphNode<Gate> gate = queue.Dequeue();
                RothEnum fault = gate.Value.sat0 ? RothEnum.Zero : RothEnum.One;
                
                PodemState st= Podem(gate, fault);
                
                if (st == PodemState.Success) 
                {
                    textBox1.AppendText("For "+gate.Value.Name+", fault="+fault.ToString()+" \n");
                    
                }
                FaultAktivated = false;
                implicationstack.Clear();
                Dfront.Clear();
            }
            EndTime = DateTime.Now;
            textBox2.AppendText("time="+(EndTime-StartTime)+ " \n");
                    
            
            
            
        }

        private PodemState Podem(GraphNode<Gate> gate, RothEnum fault)
        {

            while (!FaultEffectAtPO())
            {
                if (XPathCheck())/* Returns true if test possible because
                                            X-path exists from D-frontier to POs */
                {
                    Target target = new Target();
                    Target PItarget = new Target();
                    ImplicationStackItem s = new ImplicationStackItem();


                    target = Objective(gate, fault); if (target.Input == null) return PodemState.Processing;
                    try
                    {
                        PItarget = Backtrace(target); /* Find the PI to set */
                    }
                    catch (InputSettingException)
                    {
                        return PodemState.Conflict;
                    }
                   

                    s.AlternativeTried = false;
                    s.Signal = PItarget.Input;
                    s.Value = PItarget.Value;

                    implicationstack.Push(s);

                    Imply(PItarget); /* Assign the value to the PI
                                        and Compute all forward implications */
                    if (Podem(gate, fault) == PodemState.Success) return PodemState.Success;
                    /* We need to backtrack */
                    else
                    {
                        PItarget = Backtrack();/* Return alternate assignment to try */
                        
                        Imply(PItarget);
                        if (Podem(gate, fault) == PodemState.Success) return PodemState.Success;
                        else
                        {
                            PItarget.Value = RothEnum.X;
                            Imply(PItarget); /* Mark pi as unknown */
                            return PodemState.Failed;
                        }
                    }
                }
                else if (implicationstack.Peek().AlternativeTried)/* no more possibilities */
                {
                    return PodemState.Failed;

                }
                else Backtrack();
            }
            return PodemState.Success;
        }

        private GraphNode<Gate> findgate(Elem elem)
        {GraphNode<Gate> d=new GraphNode<Gate>();
            foreach (GraphNode<Gate> s in Graph.Nodes)
        {
            if (s.Value.ArOut.Name != elem.Name) continue;
            else
            { d=s; }
        }
            return d;
        }

        private bool XPathCheck()
        {
            if (!FaultAktivated)
            {
                return true;
            }
           if (Dfront.Count() <= 0)
            {
                return false;
            }
            return true;
        }

        private Target Backtrack()
        {

            ImplicationStackItem cut = implicationstack.Pop();
            if (!cut.AlternativeTried)
            {
                cut.Value = !cut.Value;
                cut.AlternativeTried = true;
                implicationstack.Push(cut);
                return new Target(cut.Signal, cut.Value);
            }
            else 
            {
                cut = implicationstack.Pop();
                return new Target(cut.Signal, cut.Value);
            }
        }

        private Target Backtrace(Target target)
        {
            GraphNode<Gate> gate = findgate(target.Input);
            Target v = target;

            while (v.Input.Type != EntityType.Input) 
            {
               // if (RequiresInversion(gate)) v.Value = !v.Value;

                if (RequiresSettingAllInputs(gate.Value.Oper, v.Value))
                {
                    //Select unassigned input a of gate s with hardest controllability to value v;
                    v.Input = HardestToControl(gate, v.Value);
                    v.Value = NonConrollingInputValues[gate.Value.Oper];
                }
                else if (gate.Value.Oper == EntityType.Not)
                {
                    v.Input = EasiestToControl(gate, v.Value);
                }
                else if (gate.Value.Oper == EntityType.Buff)
                {
                    v.Input = EasiestToControl(gate, v.Value);
                }
                else
                {
                    //Select unassigned input a of gate s with easiest controllability to value v;
                    v.Input = EasiestToControl(gate, v.Value);
                    v.Value = ControllingInputValues[gate.Value.Oper];
                }
                
                foreach(GraphNode<Gate> w in gate.Neighbors2)
                {
                    if (w.Value.ArOut.Name != v.Input.Name) continue;
                    gate = w;
                }
            }
            return v;
        }

        private Target Objective(GraphNode<Gate> gate, RothEnum fault)
        {
            RothValue q= fault;
            /*Target fault gate g stuck-at v */
            if (!FaultAktivated) { FaultAktivated = true; return new Target(gate.Value.ArOut, !q); }

                //Not sure work right
            else
            {
                GraphNode<Gate> P = new GraphNode<Gate>();
                if (Dfront != null)
                {
                    P = Dfront.First();
                }
                //foreach (GraphNode<Gate> w in Dfront.OrderByDescending(f=>f.Value.Level))
                //{
                //    if (w.Value.ArIn.Exists(s => s.Name == gate.Value.ArOut.Name)) P = w;
                //}
                if (P.Value != null)
                {
                    Elem unassigmentInpt = P.Value.ArIn.Find(s => s.Value == RothEnum.X);
                    q = ControllingInputValues[P.Value.Oper];
                    return new Target(unassigmentInpt, !q);
                }
                return new Target();
                

            }
        }

        private void UpdateDFrontier()
        {
            var df = from enties in Graph.Nodes
                     where enties.Value.ArIn.Exists(s => s.Value == RothEnum.D || s.Value == RothEnum.NotD)
                     &&(enties.Value.ArOut.Value==RothEnum.X)
                     select enties;
            Dfront.Clear();
            foreach (GraphNode<Gate> s in df) 
            {
                Dfront.Add(s);
            }
        }

        private void showINpts(Graph<Gate> Graph)
        {

            Graph.Nodes.OrderBy(s=>s.Value.Level);
            textBox1.AppendText("Inputs: \n");
            foreach (GraphNode<Gate> q in Graph.Nodes)
            {
                
                foreach (Elem s in q.Value.ArIn) 
                {
                    if (s.Type != EntityType.Input) continue;
                    textBox1.AppendText(s.Name+" = "+s.Value.ToString()+"\n");
                    
                }
            }
            textBox1.AppendText("Outputs: \n");
            foreach (GraphNode<Gate> t in Graph.Nodes)
            {

               
                if (t.Value.ArOut.Type != EntityType.Out) continue; 
                textBox1.AppendText(t.Value.ArOut.Name + " = " + t.Value.ArOut.Value.ToString() + "\n");
            }
        }

        private bool RequiresInversion(GraphNode<Gate> objec)
        {
            if (objec.Value.Oper == EntityType.Nand || objec.Value.Oper == EntityType.Nor || objec.Value.Oper == EntityType.Not) { return true; }
            return false;
        }

        private Elem EasiestToControl(GraphNode<Gate> entity, RothEnum val)
        {
            try
            {
                return (val == RothEnum.Zero) ?
                entity.Value.ArIn.OrderBy(i => i.CC0).First(s => s.Value == RothEnum.X) :
                entity.Value.ArIn.OrderBy(i => i.CC1).First(s => s.Value == RothEnum.X);
            }
            catch (Exception)
            {
                
                throw new InputSettingException ();
            }
            
        }

        private Elem HardestToControl(GraphNode<Gate> entity, RothEnum val)
        {
            try
            {
                return (val == RothEnum.Zero) ?
                entity.Value.ArIn.OrderByDescending(i => i.CC0).First(s => s.Value == RothEnum.X) :
                entity.Value.ArIn.OrderByDescending(i => i.CC1).First(s => s.Value == RothEnum.X);
            }
            catch (Exception)
            {
                
                throw new InputSettingException ();
            }
            
        }

        private bool RequiresSettingAllInputs(EntityType entityType, RothEnum val)
        {
            if (entityType==EntityType.Not||HardControlledOutputValues[entityType] != val) return false;
            return true;
        }

        private void Imply(Target tgt)
        {
            Graph.Nodes.OrderBy(s => s.Value.Level);

            foreach (GraphNode<Gate> gateNode in Graph.Nodes)
            {
                int count = gateNode.Value.ArIn.Count;
                
                Elem[] inpts = new Elem[count];
                for (int r = 0; r < count; r++)
                {
                    
                    inpts[r] = gateNode.Value.ArIn[r];
                    if (inpts[r].Name == tgt.Input.Name) inpts[r].Value = tgt.Value;

                }
                RothEnum w = RothEnum.X;

                for (int e = 0; e < count; e++)
                {
                    if (e == 0) { w = inpts[0].Value.Value; }
                    else
                    {
                        w = geek(w, inpts[e].Value.Value, gateNode.Value.Oper);
                    }

                }
                
                if(gateNode.Value.sat1)
                {
                    if (w != RothEnum.Zero) FaultAktivated = false;
                Graph.Nodes.FindByValue(gateNode.Value).Value.ArOut.Value = RothEnum.NotD;
                }
                else if (gateNode.Value.sat0)
                {
                    if (w != RothEnum.One) FaultAktivated = false;
                    Graph.Nodes.FindByValue(gateNode.Value).Value.ArOut.Value = RothEnum.D;
                }
                else { Graph.Nodes.FindByValue(gateNode.Value).Value.ArOut.Value = w; }
                foreach (GraphNode<Gate> x in gateNode.Neighbors)
                {
                    foreach (Elem s in x.Value.ArIn)
                    {
                        if (gateNode.Value.ArOut.Name != s.Name) continue;
                        gateNode.Neighbors.FindByValue(x.Value).Value.ArIn.Find(r => r.Name == s.Name).Value = gateNode.Value.ArOut.Value;
                    }

                }
            }
            UpdateDFrontier();
        }
        private bool FaultEffectAtPO()
        {
            var posWithFaultEffect =
                 from sUnit in Graph.Nodes
                 where sUnit.Value.ArOut.Type == EntityType.Out && (sUnit.Value.ArOut.Value == RothEnum.D || sUnit.Value.ArOut.Value==RothEnum.NotD)
                 select sUnit;

            if (posWithFaultEffect.Count() <= 0 )
            {
                return false;
            }

            return true;
        }
     
       private RothEnum geek(RothEnum w, RothEnum elem, EntityType entityType)
       {
           switch (entityType) 
           {
               case EntityType.And: w = Definitions.AndRes[(int)w, (int)elem]; break;
               case EntityType.Nand: w = Definitions.AndRes[(int)w, (int)elem]; w = Definitions.NotRes[(int)w]; break;
               case EntityType.Or: w = Definitions.OrRes[(int)w, (int)elem]; break;
               case EntityType.Nor: w = Definitions.OrRes[(int)w, (int)elem]; w = Definitions.NotRes[(int)w]; break;
               case EntityType.Not: w = Definitions.NotRes[(int)w]; break;
               case EntityType.Xor: w = Definitions.XorRes[(int)w, (int)elem]; break;
           }
           return w;
       }
    
        private Queue<GraphNode<Gate>> CreateFaultQueue(Graph<Gate> Graph)
        {
            Queue<GraphNode<Gate>> queue=new Queue<GraphNode<Gate>>();
            foreach (GraphNode<Gate> gate in Graph.Nodes) 
            {
                if (gate.Value.sat0 || gate.Value.sat1) queue.Enqueue(gate);
                continue;
                
            }
                return queue;
        }

        #region Buttons
        //BTN Open scheme
        private void button3_Click(object sender, EventArgs e)
        {
            int lev = 0;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pat = openFileDialog1.FileName;
                try
                {
                    parse_Bench(pat);
                    gates = associate(glob, gates);
                    Graph = FillTree(gates);
                    Levelize(Graph);
                    
                    foreach (GraphNode<Gate> s in Graph.Nodes)
                    {
                        if (s.Value.Level > lev) lev = s.Value.Level; if (s.Value.Level == 0) textBox1.AppendText(s.Value.Name);
                    }
                    ControllabilityCalculation(Graph,lev);
                    textBox1.AppendText("\nКоличество уровней схемы:  " + lev.ToString() + "\n");
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button4.Enabled = true;
                }
                catch (Exception w) 
                {
                    textBox2.AppendText(w.Message);
                }
                Graph229 = Graph;
            }
        }
        
        //BTN Clear all
        private void button4_Click(object sender, EventArgs e)
        {
            glob.Clear();
            gates.Clear();
            Graph.Nodes.Clear();
            eqwert.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            button1.Enabled = false;
            button2.Enabled = false;
            button4.Enabled = false;
           
        }

        //BTN Input Fault
        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2(Graph);
            form.ShowDialog();
            if (form.DialogResult == DialogResult.OK) 
            {
                Graph = form.returnData();
                int count = 0;
                foreach (GraphNode<Gate> ga in Graph.Nodes)
                {
                    if (ga.Value.sat0 || ga.Value.sat1)
                    {
                        count++;
                        textBox3.AppendText(ga.Value.Name +"        s-a-"+ (ga.Value.sat0 ? "0" : "1")+"\n");
                    }
                }
                textBox1.AppendText("Внесено ошибок:    " + count + "\n");
            }
            
        }

        //BTN Exit
        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
       #endregion
        #region Controllability
        public void ControllabilityCalculation(Graph<Gate> graph, int lev)
        {

            for (int i = 1; i <= lev; i++)
            {
                foreach (GraphNode<Gate> s in graph.Nodes)
                {
                    if (s.Value.Level != i) continue;
                    else
                    {
                       
                            foreach (Elem vxod in s.Value.ArIn)
                            {
                                if (vxod.Type != EntityType.Input) continue;
                                {
                                    vxod.CC0 = 1;
                                    vxod.CC1 = 1;
                                }
                            }
                        
                        s.Value.ArOut.CC0 = findCC0(s.Value);
                        s.Value.ArOut.CC1 = findCC1(s.Value);
                        foreach (GraphNode< Gate> gat in s.Neighbors) 
                        {
                            foreach (Elem delem in gat.Value.ArIn) 
                            {
                                if (delem.Name != s.Value.ArOut.Name) continue;
                                else
                                {
                                    delem.CC0 = s.Value.ArOut.CC0;
                                    delem.CC1 = s.Value.ArOut.CC1;
                                }
                            }
                        }


                    }

                }
               
            }

            //foreach (GraphNode<Gate> s in graph.Nodes)
            //{
            //    textBox1.AppendText(s.Value.Name+"\nВходы:\n");
            //    foreach(Elem q in s.Value.ArIn)
            //    {
            //        textBox1.AppendText(q.Name + ": "+q.CC0+","+q.CC1+";\n");
            //    }
            //    textBox1.AppendText(" Выход: "+s.Value.ArOut.Name+"  "+s.Value.ArOut.CC0+","+s.Value.ArOut.CC1+"\n");
            //    textBox1.AppendText("Level of gate: "+s.Value.Level+"\n");
            //    textBox1.AppendText("---\n");
            //}
        }  
        private int findCC0(Gate gate)
        {
            int i=0;
            int[] fCC0=new int[gate.ArIn.Count];
            int[] fCC1= new int[gate.ArIn.Count];
            for(int q=0;q<gate.ArIn.Count;q++)
            {
                fCC0[q]=gate.ArIn[q].CC0;
                fCC1[q] = gate.ArIn[q].CC1;
            }
           
            //for CC0
            switch (gate.Oper)
            {
                case EntityType.And:  i = fCC0.Min() + 1; break;
                case EntityType.Nand: i = fCC1.Sum() + 1; break;
                case EntityType.Or:   i = fCC0.Sum() + 1; break;
                case EntityType.Nor:  i = fCC1.Min() + 1; break;
                case EntityType.Xor: i = Math.Min(fCC0.Sum(),fCC1.Sum())+1; break;
                case EntityType.Not: i = fCC1.Sum() + 1; break;
                case EntityType.Buff: i = fCC0.Min() + 1; break;
                default: i= -1; break;
            }
            return i;
        }
        private int findCC1(Gate gate)
        {
            int i = 0;
            int[] fCC0 = new int[gate.ArIn.Count];
            int[] fCC1 = new int[gate.ArIn.Count];
            for (int q = 0; q < gate.ArIn.Count; q++)
            {
                fCC1[q] = gate.ArIn[q].CC1;
                fCC0[q] = gate.ArIn[q].CC0;
            }
           
            //for CC1
            switch (gate.Oper)
            {
                case EntityType.And: i = fCC1.Sum() + 1; break;
                case EntityType.Nand: i = fCC0.Min() + 1; break;
                case EntityType.Or: i = fCC1.Min() + 1; break;
                case EntityType.Nor: i = fCC0.Sum() + 1; break;
                case EntityType.Xor: i = Math.Min(fCC1[0] + fCC0[1],
                                         fCC0[0] + fCC1[1]) + 1; break;
                case EntityType.Not: i = fCC0.Sum() + 1; break;
                case EntityType.Buff: i = fCC1.Min() + 1; break;
                default: i = -1; break;
            }
            return i;
        }
        #endregion
        #region Parsing
        public void parse_Verilog(string path)
        {

            string line;
            string input;
            string output;
            string wire;

            List<string> mas = new List<string>();
            List<string> d = new List<string>();



            string pattern1 = @"(?<=input)\S+(?=\;)";
            string pattern2 = @"(?<=output)\S+(?=\;)";
            string pattern3 = @"(?<=wire)\S+(?=\;)";
            string pattern4 = @"(?<key>\S+)\s(?<name>\S+)\s\((?<some>.*)\)";
            Regex regex = new Regex(pattern4);

            string linesa;
            StreamReader f = new StreamReader(path);
            StreamReader n = new StreamReader(path);

            /* Make first pass to count primary inputs, primary outputs, and wires. */
            line = f.ReadToEnd();
            linesa = ((((line.Replace("\r\n", string.Empty)).Replace(" ", "")).Replace("inputs", "")).Replace("outputs", "")).Replace(";", "; ");
            while (n.Peek() >= 0)
            {
                d.Add(n.ReadLine());
            }


            input = Regex.Match(linesa, pattern1).Value;
            output = Regex.Match(linesa, pattern2).Value;
            wire = Regex.Match(linesa, pattern3).Value;

            string[] lIn = input.Split(',');
            foreach (string m in lIn)
            {
                Elem i = new Elem();
                i.Name = m;
                i.Type = EntityType.Input;
                glob.Add(i);

            }
            string[] lOut = output.Split(',');
            foreach (string m in lOut)
            {
                Elem o = new Elem();
                o.Name = m;
                o.Type = EntityType.Out;
                glob.Add(o);
            }
            string[] lwire = wire.Split(',');
            foreach (string m in lwire)
            {
                Elem w = new Elem();
                w.Name = m;
                w.Type = EntityType.Wire;
                glob.Add(w);
            }

            //Second pass. Gettings the data
            foreach (string s in d)
            {
                if (s.StartsWith("/") == true) continue;
                if (s.StartsWith("wire") == true) continue;
                if (s.StartsWith("input") == true) continue;
                if (s.StartsWith("module") == true) continue;
                if (s.StartsWith("endmodule") == true) continue;
                if (s.StartsWith("output") == true) continue;
                if (s == string.Empty) continue;

                var matches = regex.Matches(s);
                foreach (var match in matches)
                {
                    Gate gate = new Gate();
                    List<Elem> pro = new List<Elem>();
                    List<string> t = new List<string>();
                    string prom;
                    string ls;
                    gate.Name = (match as Match).Groups["name"].Value;
                    ls=(match as Match).Groups["key"].Value;
                    gate.Oper = GetT(ls);
                    prom = (match as Match).Groups["some"].Value;
                    prom = prom.Replace(" ", "");

                    t = prom.Split(',').ToList();
                    gate.ArOut.Name = t[0];
                    gate.ArOut.Type = EntityType.Out;
                    t.RemoveAt(0);
                    foreach (string o in t)
                    {
                        Elem e = new Elem();
                        e.Name = o;
                        pro.Add(e);
                    }
                    gate.ArIn = pro;
                    gates.Add(gate);

                }
            }


        }

        public void parse_Bench(string path)
        {
            textBox2.AppendText("Parse file...");
            textBox1.AppendText("Path: " + path + "\n");
            List<string> input = new List<string>();
            List<string> output = new List<string>();
            List<string> wire = new List<string>();
            string patternIn = @"(?<=input\()\S+\)";
            string patternOut = @"(?<=output\()\S+\)";
            string patternWire = @"(?<Wire>\S+)\=(?<Option>\S+)\((?<Wires>.*)\)";
            List<string> mas = new List<string>();
            Regex myRegex1 = new Regex(patternIn, RegexOptions.IgnoreCase);
            Regex myRegex2 = new Regex(patternOut, RegexOptions.IgnoreCase);
            Regex myRegex3 = new Regex(patternWire, RegexOptions.IgnoreCase);
            DateTime time = DateTime.Now;
            StreamReader f = new StreamReader(path);

            while (f.Peek() >= 0)
            {
                mas.Add(f.ReadLine());

            }
            foreach (string lin in mas)
            {
                string lineq;

                if (lin == string.Empty) continue;
                if (lin.StartsWith("#")) { textBox1.AppendText(lin + "\n"); continue; }
                lineq = lin.Replace(" ", "");
                Match my = myRegex1.Match(lineq);
                if (my.Success) { input.Add(my.Value); }
                my = myRegex2.Match(lineq);
                if (my.Success) { output.Add(my.Value); }
                my = myRegex3.Match(lineq);
                if (my.Success) { wire.Add(my.Value); }

            }
            foreach (string line in input)
            {
                string l = line.Replace(")", "");
                Elem i = new Elem();
                i.Name = l;
                i.Type = EntityType.Input;
                glob.Add(i);
            }
            foreach (string line in output)
            {
                string l = line.Replace(")", "");
                Elem i = new Elem();
                i.Name = l;
                i.Type = EntityType.Out;
                glob.Add(i);
            }

            foreach (string s in wire)
            {
                var matches = myRegex3.Matches(s);
                foreach (Match match in matches)
                {
                    Gate gate = new Gate();
                    List<Elem> pro = new List<Elem>();
                    List<string> t = new List<string>();
                    string ls;
                    string prom;
                    gate.Name = (match as Match).Groups["Option"].Value + (match as Match).Groups["Wire"].Value;
                    ls = (match as Match).Groups["Option"].Value.ToLower();
                    gate.Oper = GetT(ls);
                    prom = (match as Match).Groups["Wires"].Value;
                    Elem i = new Elem();
                    i.Name = (match as Match).Groups["Wire"].Value;
                    i.Type = EntityType.Wire;
                    
                        glob.Add(i);
                    t = prom.Split(',').ToList();
                    gate.ArOut.Name = (match as Match).Groups["Wire"].Value; ;


                    foreach (string o in t)
                    {
                        Elem e = new Elem();
                        e.Name = o;
                        pro.Add(e);
                    }
                    gate.ArIn = pro;
                    gates.Add(gate);
                }
            }
            DateTime time2 = DateTime.Now;
            textBox2.AppendText((time2 - time).Seconds.ToString() + "," + (time2 - time).Milliseconds.ToString() + " сек.\n");
        }

        private EntityType GetT(string ls)
        {
            EntityType s=EntityType.None;
            switch(ls)
            {
                case "nand":s= EntityType.Nand;
                    break;
                case "and": s = EntityType.And;
                    break;
                case "or": s = EntityType.Or;
                    break;
                case "nor": s = EntityType.Nor;
                    break;
                case "xor": s = EntityType.Xor;
                    break;
                case "not": s = EntityType.Not;
                    break;
                case "buff": s = EntityType.Buff;
                    break;
            }
            return s;
        }
        #endregion      
    }
    #region Definitions
    public class InputSettingException : Exception
    {
       
        public InputSettingException()
            : base("Trying to set an assigned input:  ")
        {
            
        }

    }
    public class Target 
    {
        private Elem input;
        private RothValue value;
        public Target(Elem input, RothValue value) 
        {
            this.input = input;
            this.value = value;
        }
        public Target() 
        {
            
        }
        public Elem Input { get {return this.input; } set{this.input=value;} }
        public RothValue Value { get { return this.value; } set { this.value = value; } }
    }
    public enum PodemState
    {
        //============================================================================
        /// <summary>
        /// Основное - рабочее состояние
        /// </summary>
        Processing,


        //============================================================================
        /// <summary>
        /// Завершено - завершающее состояние ( если решение найдено )
        /// </summary>
        Success,

        //============================================================================
        /// <summary>
        /// Обнаружен конфликт.
        /// </summary>
        Conflict,
        //============================================================================
        /// <summary>
        /// Неудача - завершающее состояние ( если сбой не является тестируемым )
        /// </summary>
        Failed,

        //============================================================================

        BacktrackLimitReached

        //============================================================================
    }
   
    public static class Definitions 
    {
       
        public static readonly RothEnum[,] AndRes ={ { RothEnum.Zero,    RothEnum.Zero, RothEnum.Zero,  RothEnum.Zero, RothEnum.Zero },
                                                     { RothEnum.Zero,    RothEnum.One,  RothEnum.D,     RothEnum.NotD, RothEnum.X },
                                                     { RothEnum.Zero,    RothEnum.D,    RothEnum.D,     RothEnum.Zero, RothEnum.X },
                                                     { RothEnum.Zero,    RothEnum.NotD, RothEnum.Zero,  RothEnum.NotD, RothEnum.X },
                                                     { RothEnum.Zero,    RothEnum.X,    RothEnum.X,     RothEnum.X,    RothEnum.X }};

        public static readonly RothEnum[,] OrRes ={ {  RothEnum.Zero,   RothEnum.One, RothEnum.D,   RothEnum.NotD, RothEnum.X },
                                                     { RothEnum.One,    RothEnum.One, RothEnum.One, RothEnum.One,  RothEnum.One },
                                                     { RothEnum.D,      RothEnum.One, RothEnum.D,   RothEnum.One,  RothEnum.X },
                                                     { RothEnum.NotD,   RothEnum.One, RothEnum.One, RothEnum.NotD, RothEnum.X },
                                                     { RothEnum.X,      RothEnum.One, RothEnum.X,   RothEnum.X,    RothEnum.X }};

        public static readonly RothEnum[]  NotRes = { RothEnum.One, RothEnum.Zero, RothEnum.NotD, RothEnum.D, RothEnum.X };

        public static readonly RothEnum[,] XorRes = { { RothEnum.Zero,    RothEnum.One,  RothEnum.D,    RothEnum.NotD,  RothEnum.X },
                                                     { RothEnum.One,      RothEnum.Zero, RothEnum.NotD, RothEnum.D,     RothEnum.X },
                                                     { RothEnum.D,        RothEnum.NotD, RothEnum.Zero, RothEnum.One,   RothEnum.X },
                                                     { RothEnum.NotD,     RothEnum.D,    RothEnum.One,  RothEnum.Zero,  RothEnum.X },
                                                     { RothEnum.X,        RothEnum.X,    RothEnum.X,    RothEnum.X,     RothEnum.X }};
        public static readonly RothEnum[] BuffRes = { RothEnum.Zero, RothEnum.One, RothEnum.D, RothEnum.NotD, RothEnum.X };
    }
    public struct RothValue
    {
        private readonly RothEnum value;
        public RothValue(RothEnum value)
        {
            this.value = value;
        }
        public RothEnum Value
        {
            get
            {
                return value;
            }
        }
        //============================================================================
        public static implicit operator RothValue(RothEnum rvalue)
        {

            var result = new RothValue(rvalue);
            return result;
        }

        //============================================================================
        public static implicit operator RothEnum(RothValue rvalue)
        {
            return rvalue.Value;
        }
        public RothValue Inversion
        {
            get
            {
                return !this;
            }
        }
        public override Boolean Equals(Object obj)
        {
            if (obj.GetType() != typeof(RothValue))
            {
                return false;
            }

            return (value == ((RothValue)obj).value);
        }
        public static RothValue operator !(RothValue rvalue)
        {
            return new RothValue(DInv[(Int32)rvalue.value]);
        }
        private static readonly RothEnum[] DInv = { RothEnum.One, RothEnum.Zero, RothEnum.X, RothEnum.NotD, RothEnum.D };
        public override String ToString()
        {
            return Enum.GetName(typeof(RothEnum), value);
        }
    }
   
    public class ImplicationStackItem 
    {

        private Elem signal;
        private RothValue value;
        private bool alternativeTried;

        public Elem Signal { get { return this.signal; } set { this.signal = value; } }
        public RothValue Value { get { return this.value; } set { this.value = value; } }
        public bool AlternativeTried { get { return this.alternativeTried; } set { this.alternativeTried = value; } }


    }
    public class Gate 
    {
        private string name;
        
        private List<Elem> arIn = new List<Elem>();
        private Elem arOut = new Elem();
        private int level;
        private bool Sat0 = false;
        private bool Sat1 = false;
        private EntityType oper { get; set; }
        

        public bool sat0 { get { return this.Sat0; } set { this.Sat0 = value; } }
        public bool sat1 { get { return this.Sat1; } set { this.Sat1 = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public EntityType Oper { get { return this.oper; } set { this.oper = value; } }
        public List<Elem> ArIn { get { return this.arIn; } set { this.arIn = value; } }
        public Elem ArOut { get { return this.arOut; } set { this.arOut = value; } }
        public int Level { get { return this.level; } set { this.level = value; } }
        
    }
    [Flags]
    public enum EntityType
    {
        None = 0xFFF,
        And = 0x001,
        Nand = 0x101,
        Or = 0x002,
        Nor = 0x102,
        Not = 0x100,
        Wire = 0x000,
        Out = 0x004,
        Input = 0x008,
        Xor = 0x020,
        Buff = 0x120
    }

    public class Elem 
    {
        private Int32 cc0;
        private Int32 cc1;
        public EntityType Type { get; set; }
        public string Name { get; set; }
        public RothValue Value { get; set; }
        public int Level { get; set; }
        public Int32 CC0{get { return this.cc0; } set { this.cc0 = value; }}
        public Int32 CC1{get { return this.cc1; } set { this.cc1 = value; }}
    }

    public enum RothEnum
    {
        //============================================================================
        Zero = 0,
        One = 1,
        D = 2,
        NotD = 3,
         X = 4
        //============================================================================
    }
    public class Graph<T> : IEnumerable<T>
    {
        private NodeList<T> nodeSet;
       
        public Graph() : this(null) { }
        public Graph(NodeList<T> nodeSet)
        {
            if (nodeSet == null)
                this.nodeSet = new NodeList<T>();
            else
                this.nodeSet = nodeSet;
        }

        public void AddNode(GraphNode<T> node)
        {
            // adds a node to the graph
            nodeSet.Add(node);
        }
        
        public void AddNode(T value)
        {
            // adds a node to the graph
            nodeSet.Add(new GraphNode<T>(value));
        }

        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to)
        {
            from.Neighbors.Add(to);
            to.Neighbors2.Add(from);
            
        }

       
        public bool Contains(T value)
        {
            return nodeSet.FindByValue(value) != null;
        }

        public bool Remove(T value)
        {
            // first remove the node from the nodeset
            GraphNode<T> nodeToRemove = (GraphNode<T>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (GraphNode<T> gnode in nodeSet)
            {
                int index = gnode.Neighbors.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.Neighbors.RemoveAt(index);
                    
                }
            }

            return true;
        }

        public NodeList<T> Nodes
        {
            get
            {
                return nodeSet;
            }
        }

        public int Count
        {
            get { return nodeSet.Count; }
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (Node<T> s in nodeSet)
            {
                yield return s.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
         throw new NotImplementedException();   
        }
    }
    
    public class GraphNode<T> : Node<T>
    {
        

        public GraphNode() : base() { }
        public GraphNode(T value) : base(value) { }
        public GraphNode(T value, NodeList<T> neighbors) : base(value, neighbors) { }
        
        new public NodeList<T> Neighbors
        {
            get
            {
                if (base.Neighbors == null)
                    base.Neighbors = new NodeList<T>();

                return base.Neighbors;
            }
        }
        new public NodeList<T> Neighbors2
        {
            get
            {
                if (base.Neighbors2 == null)
                    base.Neighbors2 = new NodeList<T>();

                return base.Neighbors2;
            }
        }

        
    }
    
    public class Node<T>
{
        // Private member-variables
        private T data;
        private NodeList<T> neighbors = null;
        private NodeList<T> neighbors2 = null;

        public Node() {}
        public Node(T data) : this(data, null) {}
        public Node(T data, NodeList<T> neighbors)
        {
            this.data = data;
            this.neighbors = neighbors;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        protected NodeList<T> Neighbors
        {
            get
            {
                return neighbors;
            }
            set
            {
                neighbors = value;
            }
        }
        protected NodeList<T> Neighbors2
        {
            get
            {
                return neighbors2;
            }
            set
            {
                neighbors2 = value;
            }
        }
    }
    
    public class NodeList<T> : Collection<Node<T>>
{
    public NodeList() : base() { }

    public NodeList(int initialSize)
    {
        // Add the specified number of items
        for (int i = 0; i < initialSize; i++)
            base.Items.Add(default(GraphNode<T>));
        
    }

    public Node<T> FindByValue(T value)
    {
        // search the list for the value
        foreach (GraphNode<T> node in Items)
            if (node.Value.Equals(value))
                return node;

        // if we reached here, we didn't find a matching node
        return null;
    }
}
#endregion

}

