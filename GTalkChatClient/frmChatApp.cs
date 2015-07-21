using System;
using System.Windows.Forms;
using agsXMPP;

namespace GTalkChatClient
{
    public partial class FrmChatApp : Form
    {
        private XmppClientConnection xmppCon;

        public FrmChatApp()
        {
            InitializeComponent();
            xmppCon = new XmppClientConnection();
        }

        public FrmChatApp( XmppClientConnection xmppCon )
        {
            InitializeComponent();
            this.xmppCon = xmppCon;
            init();
        }

        private void init()
        {
            listChats.Items.Clear();
            xmppCon.OnRosterStart += OnRosterStart;
            xmppCon.OnRosterEnd += OnRosterEnd;
            xmppCon.OnRosterItem += OnRosterItem;
            xmppCon.OnPresence += OnPresence;
            xmppCon.OnError += OnError;
            xmppCon.OnClose += OnClose;
            xmppCon.OnMessage += OnMessage;
        }

        private void OnMessage( object sender, agsXMPP.protocol.client.Message msg )
        {
            if ( msg.Body == null )
            {
                return;
            }

            if ( InvokeRequired )
            {
                BeginInvoke( new agsXMPP.protocol.client.MessageHandler( OnMessage ), sender, msg );
                return;
            }

            //listChats.Items.Add( String.Format( "{0} type: {1}", msg.From.Bare, msg.Type ) );
            listChats.Items.Add( "< " + msg.From.Bare + " >  " + msg.Body );
            listChats.SelectedIndex = listChats.Items.Count - 1;
        }

        private void OnClose( object sender )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new ObjectHandler( OnClose ), sender );
            }
            ;
        }

        private void OnError( object sender, Exception ex )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new ErrorHandler( OnError ), sender, ex );
                return;
            }
            listChats.Items.Add( "Error" );
            listChats.SelectedIndex = listChats.Items.Count - 1;
        }

        private void OnPresence( object sender, agsXMPP.protocol.client.Presence pres )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new agsXMPP.protocol.client.PresenceHandler( OnPresence ), sender, pres );
            }
            //listChats.Items.Add( String.Format( "Received Presence from:{0} show:{1} status:{2}", pres.From, pres.Show,
            //    pres.Status ) );
            //listChats.SelectedIndex = listChats.Items.Count - 1;
        }

        private void OnRosterItem( object sender, agsXMPP.protocol.iq.roster.RosterItem item )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new XmppClientConnection.RosterHandler( OnRosterItem ), sender, item );
                return;
            }
            listBuddyList.Items.Add( String.Format( "{0}", item.Jid.Bare ) );
            listBuddyList.SelectedIndex = listChats.Items.Count - 1;
        }

        private void OnRosterEnd( object sender )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new ObjectHandler( OnRosterEnd ), sender );
                return;
            }
            xmppCon.SendMyPresence();
        }

        private void OnRosterStart( object sender )
        {
            if ( InvokeRequired )
            {
                BeginInvoke( new ObjectHandler( OnRosterStart ), sender );
            }
        }

        private void btnSend_Click( object sender, EventArgs e )
        {
            sendMessage( txtMessage.Text, listBuddyList.SelectedItem.ToString() );
        }

        public void sendMessage( string msg, string receiverName )
        {
            xmppCon.Send( new agsXMPP.protocol.client.Message( new Jid( receiverName ),
                agsXMPP.protocol.client.MessageType.chat, msg ) );
            listChats.Items.Add( "< " + xmppCon.MyJID.Bare + " >  " + txtMessage.Text );
            listChats.SelectedItem = listChats.Items.Count - 1;
            txtMessage.Text = "";
        }

        private void frmChatApp_FormClosing( object sender, FormClosingEventArgs e )
        {
            Application.Exit();
        }
    }
}