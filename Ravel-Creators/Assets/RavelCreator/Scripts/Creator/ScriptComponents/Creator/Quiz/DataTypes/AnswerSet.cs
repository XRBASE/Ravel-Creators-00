using System;
using UnityEngine;

namespace Base.Ravel.Creator.Components.Quiz
{
    /// <summary>
    /// Generic set of answers, so any type can be constructed and selected in multiple choice situations.
    /// The multi choice answer class does have to be overwritten for new types, to implement showing and selection behaviour for these new types.
    /// </summary>
    [Serializable]
    public abstract class AnswerSet<T> : AnswerSet
    {
        /// <summary>
        /// Retrieves one of the answers in object form, so items can be accessed from within the non generic parent class.
        /// </summary>
        /// <param name="i">index of answer that is retrieved.</param>
        public override object this[int i] {
            get { return _answers[i]; }
        }
        
        /// <summary>
        /// Amount of available answers
        /// </summary>
        public override int Length {
            get { return _answers.Length; }
        }
        
        [SerializeField] private T[] _answers;

        public AnswerSet()
        {
            _answers = Array.Empty<T>();
        }
    }

    /// <summary>
    /// Set of answers used for multiple choice questions
    /// </summary>
    [Serializable]
    public abstract class AnswerSet
    {
        /// <summary>
        /// Get answer with given index cast into object.
        /// </summary>
        /// <param name="i">index of answer.</param>
        public abstract object this[int i] { get; }
        
        public virtual int Length {
            get { return 0; }
        }

        /// <summary>
        /// Returns new inheritor instance for anmswerset of given type.
        /// </summary>
        /// <param name="t">type of answers to expect in the set.</param>
        public static AnswerSet GetSetOfType(Type t)
        {
            switch (t) {
                case Type.None:
                    return null;
                case Type.String:
                    return new StringAnswerSet();
                case Type.Sprite:
                    return new SpriteAnswerSet();
                case Type.GameObject:
                    return new GameObjectAnswerSet();
                default:
                    throw new Exception($"Implementation for answerset of type {t} is missing!");
            }
        }

        /// <summary>
        /// Type used for determining the type of answer and corresponding classes to match to this type of answer.
        /// </summary>
        public enum Type
        {
            None = 0,
            String = 1,
            Sprite = 2,
            GameObject = 3,
        }
    }

    //These classes are needed to make the inspector of unity able to display and serialize the classes, as it can't
    //serialize references of generic classes (because its type is not a given in that instance). None of them have to 
    //implement any body though.
    
    public class StringAnswerSet : AnswerSet<string> { }
    public class SpriteAnswerSet : AnswerSet<Sprite> { }
    public class GameObjectAnswerSet : AnswerSet<GameObject> { }
}