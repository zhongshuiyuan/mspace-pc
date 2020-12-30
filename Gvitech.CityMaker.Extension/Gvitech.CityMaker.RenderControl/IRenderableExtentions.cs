using System;

namespace Gvitech.CityMaker.RenderControl
{
    public static class IRenderableExtentions
    {
        public static T SetVisibleParam<T>(this T renderable, double maxVisibleDistance = 100000.0, float minVisiblePixels = 15f, gviViewportMask vmask = gviViewportMask.gviViewAllNormalView, gviDepthTestMode dtm = gviDepthTestMode.gviDepthTestEnable, double viewingDistance = 0.0, double minVisibleDistance = -100000.0) where T : IRenderable
        {
            bool flag = renderable == null;
            T result;
            if (flag)
            {
                result = default(T);
            }
            else
            {
                renderable.MaxVisibleDistance = maxVisibleDistance;
                renderable.MinVisiblePixels = minVisiblePixels;
                renderable.MinVisibleDistance = minVisibleDistance;
                renderable.ViewingDistance = ((viewingDistance <= 0.0) ? ((double)renderable.Envelope.DiagonalDistance() * 1.5) : viewingDistance);
                renderable.DepthTestMode = dtm;
                renderable.VisibleMask = vmask;
                result = renderable;
            }
            return result;
        }

        /// <summary>
        /// ���ö���ͼ��ɼ�״̬
        /// </summary>
        /// <param name="renderable">ͼ��</param>
        /// <param name="viewportMode">����״̬</param>
        /// <param name="viewPortIndex">ͼ���ӿ���ţ�����״̬Ϊ15,����״̬��Ϊ�ӿ���ţ���һ��Ϊ0���ڶ�Ϊ1����������</param>
        /// <param name="isVisilbe">�Ƿ�ɼ�</param>
        public static void SetVisibleMask(this IRenderable renderable, gviViewportMode viewportMode, int viewPortIndex, bool isVisilbe)
        {
            if (viewportMode == gviViewportMode.gviViewportSinglePerspective)//����״̬
            {
                renderable.VisibleMask = isVisilbe ? gviViewportMask.gviViewAllNormalView : gviViewportMask.gviViewNone;
            }
            else if (viewportMode == gviViewportMode.gviViewportL1R1)// ����˫��״̬
            {
                SetL1R1VisibleMask(renderable, viewPortIndex, isVisilbe);
            }
            else if (viewportMode == gviViewportMode.gviViewportL1M1R1)//����������
            {
                SetL1M1R1VisibleMask(renderable, viewPortIndex, isVisilbe);
            }
            else if (viewportMode == gviViewportMode.gviViewportQuad)//����
            {
                SetQuadVisibleMask(renderable, viewPortIndex, isVisilbe);
            }
        }

        /// <summary>
        /// ����˫��״̬
        /// </summary>
        /// <param name="renderable"></param>
        /// <param name="viewPortIndex">ͼ���ӿ���ţ�����״̬Ϊ15,����״̬��Ϊ�ӿ���ţ���һ��Ϊ0���ڶ�Ϊ1����������</param>
        /// <param name="isVisilbe">�Ƿ�ɼ�</param>
        private static void SetL1R1VisibleMask(IRenderable renderable, int viewPortIndex, bool isVisilbe)
        {
            if (viewPortIndex == 0)//��һ��
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView0;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView1;
                    else
                    {
                        SetIndexUnVisible(renderable, gviViewportMask.gviView0);
                    }
                }
            }
            else if (viewPortIndex == 1)
            {
                if (isVisilbe)
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView1;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView1;
                    else
                    {
                        SetIndexUnVisible(renderable, gviViewportMask.gviView1);
                    }
                }
            }
        }

        private static void SetIndexUnVisible(IRenderable renderable, gviViewportMask viewIndex)
        {
            var value = (int)renderable.VisibleMask;
            var index = (int)viewIndex;
            if ((value & index) == index)//��һ���ɼ�
            {
                renderable.VisibleMask = renderable.VisibleMask ^ viewIndex;
            }
        }




        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="renderable"></param>
        /// <param name="viewPortIndex">ͼ���ӿ���ţ�����״̬Ϊ15,����״̬��Ϊ�ӿ���ţ���һ��Ϊ0���ڶ�Ϊ1����������</param>
        /// <param name="isVisilbe">�Ƿ�ɼ�</param>
        private static void SetL1M1R1VisibleMask(IRenderable renderable, int viewPortIndex, bool isVisilbe)
        {
            if (viewPortIndex == 0)//��һ��
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView0;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView1 | gviViewportMask.gviView2;
                    else
                        SetIndexUnVisible(renderable, gviViewportMask.gviView0);
                }
            }
            else if (viewPortIndex == 1)//�ڶ���
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView1;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView0 | gviViewportMask.gviView2;
                    else
                        SetIndexUnVisible(renderable, gviViewportMask.gviView1);

                }

            }
            else if (viewPortIndex == 2)//������
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView2;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView0 | gviViewportMask.gviView1;
                    else
                        SetIndexUnVisible(renderable, gviViewportMask.gviView2);

                }
            }

        }

        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="renderable"></param>
        /// <param name="viewPortIndex">ͼ���ӿ���ţ�����״̬Ϊ15,����״̬��Ϊ�ӿ���ţ���һ��Ϊ0���ڶ�Ϊ1����������</param>
        /// <param name="isVisilbe">�Ƿ�ɼ�</param>
        private static void SetQuadVisibleMask(IRenderable renderable, int viewPortIndex, bool isVisilbe)
        {
            if (viewPortIndex == 0)//��һ��
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView0;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView1 | gviViewportMask.gviView2 | gviViewportMask.gviView3;
                    else
                        SetIndexUnVisible(renderable, gviViewportMask.gviView0);

                }
            }
            else if (viewPortIndex == 1)//�ڶ���
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView1;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView0 | gviViewportMask.gviView2 | gviViewportMask.gviView3;
                    else
                        SetIndexUnVisible(renderable, gviViewportMask.gviView1);

                }

            }
            else if (viewPortIndex == 2)//������
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView2;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView0 | gviViewportMask.gviView1 | gviViewportMask.gviView3;
                    else
                        SetIndexUnVisible(renderable, gviViewportMask.gviView2);

                }
            }
            else if (viewPortIndex == 3)//������
            {
                if (isVisilbe)//�ɼ�״̬
                    renderable.VisibleMask = renderable.VisibleMask | gviViewportMask.gviView3;
                else
                {
                    if ((int)(renderable.VisibleMask) == 15 || (int)(renderable.VisibleMask) == 255)
                        renderable.VisibleMask = gviViewportMask.gviView0 | gviViewportMask.gviView1 | gviViewportMask.gviView2;
                    else
                        SetIndexUnVisible(renderable, gviViewportMask.gviView3);

                }
            }
        }


    }
}
