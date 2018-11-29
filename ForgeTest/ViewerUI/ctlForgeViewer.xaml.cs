using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForgeTest.ViewerUI
{
    public partial class ctlForgeViewer : UserControl
    {
        #region Template

        protected string m_Template =
@"<!DOCTYPE html>
<html>
<head>
	<meta charset=""UTF-8"">
	<link rel=""stylesheet"" href=""https://developer.api.autodesk.com/modelderivative/v2/viewers/style.min.css?v=v5.*"" />
	<script src=""https://developer.api.autodesk.com/modelderivative/v2/viewers/viewer3D.min.js?v=v5.*""></script>
    <style>
        *, *:before, *:after {
            box-sizing: border-box;
        }
    </style>
</head>
<body onload=""initialize()"">
<div id=""viewer"" style=""position:absolute; width:99%; height:98%; left:0.5%; top:1%;""></div>
<script>
	function authMe () { return ('__ACCESS_TOKEN__') ; }

	function initialize () {
		var options ={
			'document' : ""urn:__URN__"",
			'env': 'AutodeskProduction',
			'getAccessToken': authMe
		} ;
		var viewerElement =document.getElementById ('viewer') ;
		//var viewer =new Autodesk.Viewing.Viewer3D (viewerElement, {}) ; / No toolbar
		var viewer =new Autodesk.Viewing.Private.GuiViewer3D (viewerElement, {}) ; // With toolbar
		Autodesk.Viewing.Initializer (options, function () {
			viewer.initialize () ;
			loadDocument (viewer, options.document) ;
		}) ;
	}
	function loadDocument (viewer, documentId) {
		// Find the first 3d geometry and load that.
		Autodesk.Viewing.Document.load (
			documentId,
			function (doc) { // onLoadCallback
				var geometryItems =[] ;
				geometryItems =Autodesk.Viewing.Document.getSubItemsWithProperties (
					doc.getRootItem (),
					{ 'type' : 'geometry', 'role' : '3d' },
					true
				) ;
				if ( geometryItems.length <= 0 ) {
					geometryItems =Autodesk.Viewing.Document.getSubItemsWithProperties (
						doc.getRootItem (),
						{ 'type': 'geometry', 'role': '2d' },
						true
					) ;
				}
				if ( geometryItems.length > 0 )
					viewer.load (
						doc.getViewablePath (geometryItems [0])//,
						//null, null, null,
						//doc.acmSessionId /*session for DM*/
					) ;

                let box = new THREE.BoxGeometry(
                    10,
                    10,
                    10);


                let material = new THREE.MeshBasicMaterial(
                    {
                        color: ""blue""
                    });

                viewer.impl.scene.add(new THREE.Mesh(box, material));

        },
			function (errorMsg) { // onErrorCallback
				alert(""Load Error: "" + errorMsg) ;
			}//,
			//{
            //	'oauth2AccessToken': authMee (),
            //	'x-ads-acm-namespace': 'WIPDM',
            //	'x-ads-acm-check-groups': 'true',
        	//}
		) ;
	}
</script>
</body>
</html>";

        #endregion

        private string m_AccessToken = "";
        private string m_URNEncoded = "";
        
        public ctlForgeViewer()
        {
            InitializeComponent();
        }

        public void SetAccessToken(string token)
        {
            m_AccessToken = token;
        }
        public void LoadURN(string urnEncoded)
        {
            string tmp;

            m_URNEncoded = urnEncoded;

            tmp = GetHTML();

            browser.LoadHtml(tmp);
        }

        public string GetHTML()
        {
            return m_Template
                .Replace("__ACCESS_TOKEN__", m_AccessToken)
                .Replace("__URN__", m_URNEncoded);
        }
    }
}
