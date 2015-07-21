using System;
using System.Windows.Forms;
using agsXMPP;

namespace GTalkChatClient
{
    public partial class FrmLogin : Form
    {
        private XmppClientConnection xmppCon;
        private Jid jidUser;

        public FrmLogin()
        {
            InitializeComponent();

            xmppCon = new XmppClientConnection();

            xmppCon.OnLogin += xmppCon_OnLogin;
            xmppCon.OnAuthError += xmppCon_OnAuthError;
            xmppCon.OnError += xmppCon_OnError;

            txtPassword.UseSystemPasswordChar = true;
        }

        private void xmppCon_OnError( object sender, Exception ex )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new ErrorHandler( xmppCon_OnError ), sender, ex );
                return;
            }
            picLoader.Visible = false;
            MessageBox.Show( "Error occured.", "GTalkCC" );
        }

        private void xmppCon_OnAuthError( object sender, agsXMPP.Xml.Dom.Element e )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new XmppElementHandler( xmppCon_OnAuthError ), sender, e );
                return;
            }
            picLoader.Visible = false;
            MessageBox.Show("Username and/or password is invalid.", "GTalkCC");
        }

        private void xmppCon_OnLogin( object sender )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new ObjectHandler( xmppCon_OnLogin ), sender );
                return;
            }
            FrmChatApp app = new FrmChatApp( xmppCon );
            app.Show();
            picLoader.Visible = false;
            Hide();
        }

        private void frmLogin_Load( object sender, EventArgs e )
        {

        }

        private void btnLogin_Click( object sender, EventArgs e )
        {
            jidUser = new Jid( txtUsername.Text );

            xmppCon.Username = jidUser.User;
            xmppCon.Server = jidUser.Server;
            xmppCon.Password = txtPassword.Text;
            xmppCon.AutoResolveConnectServer = true;

            picLoader.Visible = true;

            xmppCon.Open();
        }

        private void btnExit_Click( object sender, EventArgs e )
        {
            Application.Exit();
        }
    }
}