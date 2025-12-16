//MIT License

//Copyright(c) 2019 Antony Vitillo(a.k.a. "Skarredghost")

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plugins.CurvedTextMeshPro.Scripts
{
    /// <summary>
    /// Class for drawing a Text Pro text following a circle arc
    /// </summary>
    [ExecuteInEditMode]
    public class TextProOnACircle : TextProOnACurve
    {
        /// <summary>
        /// The radius of the text circle arc
        /// </summary>
        [FormerlySerializedAs("m_radius")]
        [SerializeField]
        [Tooltip("The radius of the text circle arc")]
        private float mRadius = 10.0f;

        /// <summary>
        /// How much degrees the text arc should span
        /// </summary>
        [FormerlySerializedAs("m_arcDegrees")]
        [SerializeField]
        [Tooltip("How much degrees the text arc should span")]
        private float mArcDegrees = 90.0f;

        /// <summary>
        /// The angular offset at which the arc should be centered, in degrees.
        /// -90 degrees means that the text is centered on the heighest point
        /// </summary>
        [FormerlySerializedAs("m_angularOffset")]
        [SerializeField]
        [Tooltip("The angular offset at which the arc should be centered, in degrees")]
        private float mAngularOffset = -90;

        /// <summary>
        /// How many maximum degrees per letters should be. For instance, if you specify
        /// 10 degrees, the distance between the letters will never be superior to 10 degrees.
        /// It is useful to create text that gracefully expands until it reaches the full arc,
        /// without making the letters to sparse when the string is short
        /// </summary>
        [FormerlySerializedAs("m_maxDegreesPerLetter")]
        [SerializeField]
        [Tooltip("The maximum angular distance between letters, in degrees")]
        private float mMaxDegreesPerLetter = 360;

        /// <summary>
        /// Previous value of <see cref="mRadius"/>
        /// </summary>
        private float m_OldRadius = float.MaxValue;

        /// <summary>
        /// Previous value of <see cref="mArcDegrees"/>
        /// </summary>
        private float m_OldArcDegrees = float.MaxValue;

        /// <summary>
        /// Previous value of <see cref="mAngularOffset"/>
        /// </summary>
        private float m_OldAngularOffset = float.MaxValue;

        /// <summary>
        /// Previous value of <see cref="mMaxDegreesPerLetter"/>
        /// </summary>
        private float m_OldMaxDegreesPerLetter = float.MaxValue;

        /// <summary>
        /// Method executed at every frame that checks if some parameters have been changed
        /// </summary>
        /// <returns></returns>
        protected override bool ParametersHaveChanged()
        {
            //check if paramters have changed and update the old values for next frame iteration
            bool retVal = !Mathf.Approximately(mRadius, m_OldRadius) || 
                          !Mathf.Approximately(mArcDegrees, m_OldArcDegrees) || 
                          !Mathf.Approximately(mAngularOffset, m_OldAngularOffset) || 
                          !Mathf.Approximately(m_OldMaxDegreesPerLetter, mMaxDegreesPerLetter);

            m_OldRadius = mRadius;
            m_OldArcDegrees = mArcDegrees;
            m_OldAngularOffset = mAngularOffset;
            m_OldMaxDegreesPerLetter = mMaxDegreesPerLetter;

            return retVal;
        }

        /// <summary>
        /// Computes the transformation matrix that maps the offsets of the vertices of each single character from
        /// the character's center to the final destinations of the vertices so that the text follows a curve
        /// </summary>
        /// <param name="zeroToOnePos">Horizontal position of the character relative to the bounds of the box, in a range [0, 1]</param>
        /// <param name="textInfo">Information on the text that we are showing</param>
        /// <param name="charIdx">Index of the character we have to compute the transformation for</param>
        /// <returns>Transformation matrix to be applied to all vertices of the text</returns>
        protected override Matrix4x4 ComputeTransformationMatrix(float zeroToOnePos, TMP_TextInfo textInfo, int charIdx)      
        {
            //calculate the actual degrees of the arc considering the maximum distance between letters
            float actualArcDegrees = Mathf.Min(mArcDegrees, textInfo.characterCount / textInfo.lineCount * mMaxDegreesPerLetter);

            //compute the angle at which to show this character.
            //We want the string to be centered at the top point of the circle, so we first convert the position from a range [0, 1]
            //to a [-0.5, 0.5] one and then add m_angularOffset degrees, to make it centered on the desired point
            float angle = ((zeroToOnePos - 0.5f) * actualArcDegrees + mAngularOffset) * Mathf.Deg2Rad; //we need radians for sin and cos

            //compute the coordinates of the new position of the central point of the character. Use sin and cos since we are on a circle.
            //Notice that we have to do some extra calculations because we have to take in count that text may be on multiple lines
            float x0 = Mathf.Cos(angle);            
            float y0 = Mathf.Sin(angle);
            float radiusForThisLine = mRadius - textInfo.lineInfo[0].lineExtents.max.y * textInfo.characterInfo[charIdx].lineNumber;
            Vector2 newMideBaselinePos = new Vector2(x0 * radiusForThisLine, -y0 * radiusForThisLine); //actual new position of the character

            //compute the trasformation matrix: move the points to the just found position, then rotate the character to fit the angle of the curve 
            //(-90 is because the text is already vertical, it is as if it were already rotated 90 degrees)
            return Matrix4x4.TRS(new Vector3(newMideBaselinePos.x, newMideBaselinePos.y, 0), Quaternion.AngleAxis(-Mathf.Atan2(y0, x0) * Mathf.Rad2Deg - 90, Vector3.forward), Vector3.one);
        }
    }
}
