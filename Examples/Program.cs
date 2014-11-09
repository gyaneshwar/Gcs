using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Examples
    {
    struct Valuetype
        {
        public int x;
        }
    public class BaseChashInDepth
        {

        public int basei;
        public BaseChashInDepth()
            {
            Console.WriteLine("This is BaseChashInDepth constructor");
            }
        public BaseChashInDepth(int i)
            {
            Console.WriteLine("This is BaseChashInDepth constructor with parameters");
            }
        public virtual void process(object x)
            {
            Console.WriteLine("base chash in depth in process");
            }
        }
    class ChashInDepth : BaseChashInDepth, ICloneable
        {
        public int referenceTypeValueType;

        public ChashInDepth()
            : base() //calling base class constructor.
            {
            Console.WriteLine("Derived class constructor");
            }

        public ChashInDepth(int i)
            : base(i) //calling base class parameterized constructor.
            {
            Console.WriteLine("Derived class constructor");
            }

        #region params 'array passing'
        public void ParamProcess(params int[] numbers)
            {
            int sum = 0;
            foreach (int item in numbers.ToList<int>())
                {
                sum += item;
                }
            Console.WriteLine("Sum of Numbers: {0}", sum);
            }
       
        #endregion

        #region special case, discovery: need to question in stack overflow.
        //note:a special case which conflicts with process overloading (params , object) ---> i have designed this to go to Object...but when i was testing with params it was going to params instead of object.
        public void process(params int[] numbers)
            {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("I am in Process params method.");
            Console.BackgroundColor = ConsoleColor.Black;
           
            }
        #endregion

        #region OUT parameter

        public void process(int x, out int y, out int z)
            {
            y = x;
            z = x + y;
            }

        #endregion

        #region Pass by Value & Reference
        public void Foo(Valuetype x)
            {
            x.x = 10;
            }

        public void Foo(ref Valuetype x)
            {
            x.x = 10;
            }

        public void Foo(BaseChashInDepth x)
            {
            x.basei = 11;
            }

        public void Foo(ref BaseChashInDepth x)
            {
            x = null;
            }
        #endregion

        #region Delegates

        public delegate void delegateinstance(int input);
        public delegate void delegateinstance1(int input);
        public delegate void delegateinstance2(int input, int input2);

        public void eventHandlerObj(object obj, EventArgs e)
            {
            Console.WriteLine("EventHandler Objects");
            }

        public void objectDelegate(int input, int input2)
            {
            Console.WriteLine("input = {0},input2 = {1}", input, input2);
            }

        public static void staticDelegate(int input)
            {
            Console.WriteLine("static Delegate : {0}", input);
            }

        public void objectDelegate(int input)
            {
            Console.WriteLine("Dynamic Delegate: {0}", input);
            }

        #endregion

        #region lack of covariance and contravariance in C#
        //lack of covariance. we were unable to override clone() method with ChashInDepth return type. public ChashInDepth Clone() would be common sense to do.
        private ChashInDepth Cloning()
            {
            return this;
            }
        public object Clone()
            {
            return this.Cloning();
            }
        //lack of parameter type contravariance.
        //'Examples.ChashInDepth.process(int)': no suitable method found to override	
        //public override void process(int x)
        //    {
        //    Console.WriteLine("implementing interfaces process method");
        //    }
        public override void process(object x)
            {
            base.process(x); // calling a base class method from the derived class method.
            Console.WriteLine("implementing BaseChashInDepth process method");
            }
        #endregion

        static void Main(string[] args)
            {
            ChashInDepth delegatesExample = new ChashInDepth();
            //(delegatesExample is a reference type)
            #region delegates
            //=================================================
            //delegates

            //creating delegates
            //C# 1.0
            //initiating delegate instance objects: both static and via reference

            delegateinstance delegateobj = new delegateinstance(delegatesExample.objectDelegate); //delegate method via object reference.
            delegateinstance delegatestatic = new delegateinstance(ChashInDepth.staticDelegate);//delegate method via static reference.
            delegateinstance duplicatedelegatestatic = delegatestatic;//creating a duplicate of delegatestatic.
            delegateinstance1 delegatestatic1 = new delegateinstance1(ChashInDepth.staticDelegate);//delegate method via static reference.
            delegateinstance2 delegateobj2 = new delegateinstance2(delegatesExample.objectDelegate);//delegate method via object reference.
            //note: delegateobj, delegatestatic1, delegatestatic, delegateobj2, duplicatedelegatestatic are all reference type objects.

            //delegateobj ----> [a]
            //delegatestatic ----> [b]
            //duplicatedelegatestatic ---->[b]
            //delegateobj2 ----> [c]
            //delegatestatic1 -----> [d]

            Console.WriteLine("Hash Code of delegateobj = {0}", delegateobj.GetHashCode());
            Console.WriteLine("Hash Code of delegatestatic = {0}", delegatestatic.GetHashCode());
            Console.WriteLine("Hash Code of duplicatedelegatestatic = {0}", duplicatedelegatestatic.GetHashCode());
            Console.WriteLine("Hash Code of delegateobj2 = {0}", delegateobj2.GetHashCode());
            Console.WriteLine("Hash Code of delegatestatic1= {0}", delegatestatic1.GetHashCode());

            /*
             * in the above study for every compilation hashvalues CHANGE for the objects.
             * --------------------------------
             * | hashkey          | hashvalue |
             * --------------------------------
             * | delegateinstance | 1111      | <----- delegateobj, delegatestatic, duplicatedelegatestatic
             * | delegateinstance1| 1122      | <----- delegatestatic1 
             * | delegateinstance2| 1133      | <----- delegateobj2
             * --------------------------------
             */

            //invoking a delegate synchronously.

            //1st method
            delegateobj(100);

            //2nd method
            delegatestatic.Invoke(100);

            //adding and deleting delegates in a series.
            delegatestatic += delegateobj; //[a] = [a] + [b] => [a,b]
            delegatestatic(100);
            delegatestatic += delegateobj; //[a,b] = [a,b] + [b] => [a,b,b]
            delegatestatic(100);
            delegatestatic -= delegatestatic; //[a,b,b] = [a,b,b] - [a,b,b] => null
            //delegatestatic(100); //this throws error since it is nothing but null
            delegatestatic += delegateobj; // [null] = [null] + [b] => [b]
            delegatestatic(100);
            delegatestatic += duplicatedelegatestatic; // [b] = [b] + [a] => [b,a]
            delegatestatic(100);

            //C# 2.0,3.0,3.5,4.0

            //1st method
            EventHandler eventhandlerInstance = new EventHandler(delegatesExample.eventHandlerObj);
            eventhandlerInstance(null, EventArgs.Empty);

            //2nd method
            EventHandler eventhandlerInstance2 = delegatesExample.eventHandlerObj;
            eventhandlerInstance2(null, EventArgs.Empty);

            //3rd method anonymous method declaration
            EventHandler eventhandlerInstance3 = delegate(object obj, EventArgs e)
                {
                    Console.WriteLine("anonymous method eventhandlerInstance3");
                };
            eventhandlerInstance3(null, EventArgs.Empty);

            //4th method anonymous methods short cut declaration
            EventHandler eventhandlerInstance4 = delegate
                {
                    Console.WriteLine("anonymous method without parameters");
                };
            eventhandlerInstance4(null, EventArgs.Empty);
            //eventhandlerInstance4(null,new MouseEventArgs(MouseButtons.None,0,0,0,0); // delegate controvariance --> MouseEventArgs overload on EventArgs.

            //5th method
            //lambda expressions - like improved anonymous methods.
            Func<int, int, String> eventhandlerInstance5 = (x, y) => (x * y).ToString();
            Console.WriteLine(eventhandlerInstance5(2, 3));
            #endregion

            #region static And Dynamic Typing
            //static typing : known at compile time.

            //explicit typing : user specifies the data type.
            int xExplicit = 10;
            Console.WriteLine(xExplicit);
            //implicit typing : compiler adapts to the data type that is assigned to it at compile time.
            var xy = 10;
            Console.WriteLine(xy.GetType().ToString());//givens Int32

            //dynamic typing : known at run time. On observation dynamic datatype takes long time for execution.

            dynamic xyz = new List<String>() { "a", "b" };
            Console.WriteLine(xyz.Count);
            xyz = "gyaneshwar";
            Console.WriteLine(xyz.Length);

            #endregion

            #region Safe code and unsafe Code
            /*
             * Safe Code: where compiler takes care of data and organizing it according to its data types. if not it will throw error.
             * Unsafe Code: where compiler doesnt take care of the data and user has to take care of it. Example : int variable can be considered as Char in C language.
             */

            #endregion

            #region Collections Strong and week C# 1.0
            //(1)
            //Arrays - strongly typed in both the language and runtime.

            //array covariance and execution time checking. Object is accessed to store the string values in the below given example.
            //first the array was declared as a string. So the array knows its a string type. that is why it throws ArrayType missmatch exception.
            String[] strings = new String[5];
            object[] objects = strings;
            //objects[0] = new Int32(); // Array type missmatch exception.
            objects[0] = "gyane"; //assigning a string doesnt throw an exception.

            //(2) (System.Collections)
            //weekly typed collection (hastable,Arraylist). These collections use objects as key value pair.
            //Arraylist can store any type of data in it. Compiler doesnt check what datatype arraylist items are referring to.

            System.Collections.ArrayList ArrayListobj = new System.Collections.ArrayList();
            ArrayListobj.Add("gyane");//adding string.
            ArrayListobj.Add(1);//adding int
            foreach (var item in ArrayListobj)
                {
                Console.WriteLine("{0} is {1} type", item, item.GetType());
                }

            //hashtable can store key, value pair in object,object pair. key and value datatype should be derivatives of Object.
            System.Collections.Hashtable hashtableobj = new System.Collections.Hashtable();
            hashtableobj.Add(1, "gyaneshwar");
            hashtableobj.Add("lol", 1);
            foreach (DictionaryEntry item in hashtableobj)
                {
                Console.WriteLine("Key: {0} is {1} type, Value: {2} is {3} type", item.Key, item.Key.GetType(), item.Value, item.Value.GetType());
                }

            //(3) (System.Collections.Specialized)
            //strongly typed collections-> example: string collections which only stores String.
            StringCollection stringobjCollection = new StringCollection();
            stringobjCollection.Add("gyaneshwar");
            stringobjCollection.Add("srinivas");
            foreach (var item in stringobjCollection)
                {
                Console.WriteLine(item);
                }
            #endregion

            #region lack of covariance in C# , a specific case is covered in this part.

            //IClonable: Object cloan();
            delegatesExample.Clone();

            //Implementing BaseChashInDepth process method.
            delegatesExample.process(null);
            //note:a special case which conflicts with process overloading (params , object) ---> i have designed this to go to parameter Datatype Object, but when i was testing with params it was going to Datatype params instead of Datatype object.
            delegatesExample.process(x: null);//this will call the specific process function with parameter Datatype as object.
            #endregion

            #region value types and reference types

            //class , delegates , array types, interface types are reference types
            //structres,enum,int,string etc are value types.

            //In Delegates Section delegatestatic, duplicatedelegatestatic are referencing to same data which is why delegates are called as reference type
            //example for reference types.

            ChashInDepth firstreference = new ChashInDepth();
            ChashInDepth secondreference = firstreference;

            firstreference.referenceTypeValueType = 1; //assigning a value to firstreference object.
            Console.WriteLine(firstreference.referenceTypeValueType == secondreference.referenceTypeValueType); // true; if true,values of firstreference is equal to secondreference.
            firstreference = null;
            Console.WriteLine(secondreference.referenceTypeValueType);//even if firstreference is made null, secondreference is pointing to the value.

            /*                               referenceTypeValueType
             *                               ----------------------
             *Step1 : firstreference ----->  |          1         |  <----- secondreference
             *                               ----------------------
             *                              
             *                                      referenceTypeValueType
             *                                      ----------------------
             *Step2 : firstreference --> null       |          1         |  <----- secondreference
             *                                      ----------------------
             * 
             */
            // example for value type
            Valuetype Valueobj = new Valuetype();
            Valuetype Valueobj1;
            Valueobj.x = 10;
            Valueobj1 = Valueobj;
            Console.WriteLine(Valueobj.x == Valueobj1.x);// checking if both the values are same.
            Valueobj.x = 11;
            Console.WriteLine(Valueobj.x == Valueobj1.x);// false, because Valueobj and Valueobj1 are holding two different values in two different memory locations.

            /*Value types store values in seperate memory individually.
             *                           x
             *                         -----
             *Step1 : valueobj ----->  | 10 |
             *                         -----
             *                           x
             *                         -----
             *Step1 : valueobj1 -----> | 10 |
             *                         -----
             *                           x
             *                         -----
             *Step1 : valueobj ----->  | 11 |
             *                         -----
             */

            //local variable values and method parameters are always stored on the stack.( This happens only for C# 1).later versions local variables will be stored in heap(anonymous methods).
            //instance variable values are always stored where ever the instance is stored. reference type instance(object) are always stored on heap as static variables.
            //Special Case:
            //Many types like (String) appear in some way to be value type, but in fact they are reference type. These are IMMUTABLE types.
            //immutable means once the instance is created it cannot be changed.

            //c# 2.0 and greater versions improvements.
            var jon = new { Name = "Gyane", age = 25 };
            var ron = new { Name = "ron", age = 45 };
            //jon ---> implicit static typing (use of var) ; new {...} is anonymous types.
            //compiler doest create a type for all of them. A blue print type will be created for similar anonymous types.
            jon = ron;
            Console.WriteLine("Equal anonymous types:{0}, jon hash code: {1} , ron hash code: {2}",jon == ron,jon.GetHashCode(),ron.GetHashCode());//true;
            
            #endregion

            #region Pass by value & reference

            BaseChashInDepth basetest = new BaseChashInDepth();
            basetest.basei = 10;
            delegatesExample.Foo(basetest);// passing referencetype by value
            Console.WriteLine(basetest.basei);// value changed from 10 to 11. Since both the references are pointing to same data.

            delegatesExample.Foo(ref basetest);//passing referencetype by reference.
            Console.WriteLine(basetest == null);// the actual reference is made null, since the reference pointing to that value was made null.

            Valuetype valuetypeobj = new Valuetype();
            valuetypeobj.x = 9;
            delegatesExample.Foo(valuetypeobj);//passing valuetype by value.
            Console.WriteLine(valuetypeobj.x);// value doesnt change from 9 to 10. it remains 9.

            delegatesExample.Foo(ref valuetypeobj);//passing valuetype by reference.
            Console.WriteLine(valuetypeobj.x);// value changed from 9 to 10

            //note:Value object sent by reference != reference object sent by value.
            /*
             * void Foo(??? Valuetype x) { x = new Valuetype(); }; }
             * ......
             * Valuetype y = new Valuetype();
             * y.i = 5;
             * Foo(??? y);
             * 
             * case1: 'Valuetype' is a value type.
             * replace ??? with Ref
             * y ends up being a new Valuetype value i.e., y.x is 0
             * 
             * case2: 'Valuetype' is a reference type.
             * replace ??? with none means "";
             * y ends up being what it is, but x will be pointing out to different location (memory).
             * 
             * observing case1 and case2 we can say that
             * Value object sent by reference != reference object sent by value.
             */
            #endregion

            #region OUT parameter

            int inx = 10;
            int outy, outz;

            delegatesExample.process(inx, out outy, out outz);
            Console.WriteLine("out y: {0} , out z: {1}", outy, outz);

            #endregion

            #region params 'array passing'

            delegatesExample.ParamProcess(4, 5, 6);
            int[] paramsx = { 1, 2, 3 };
            delegatesExample.ParamProcess(paramsx);
            //delegatesExample.process(1, 2, 3, paramsx); //a combination of both will not be possible.

            #endregion

            Console.Read();
            }
        }

    }
