
namespace WindNight.DataSourceTestTool.Forms
{
    partial class DataSourceTestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cb_DbType = new System.Windows.Forms.ComboBox();
            this.lbl_db = new System.Windows.Forms.Label();
            this.btn_Test = new System.Windows.Forms.Button();
            this.tb_ConnStr = new System.Windows.Forms.TextBox();
            this.tb_Output = new System.Windows.Forms.TextBox();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.cb_IsAck = new System.Windows.Forms.CheckBox();
            this.tb_RoutingKey = new System.Windows.Forms.TextBox();
            this.tb_SendMsg = new System.Windows.Forms.TextBox();
            this.btn_Start = new System.Windows.Forms.Button();
            this.cb_ClearOutput = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cb_DbType
            // 
            this.cb_DbType.FormattingEnabled = true;
            this.cb_DbType.Location = new System.Drawing.Point(102, 21);
            this.cb_DbType.Name = "cb_DbType";
            this.cb_DbType.Size = new System.Drawing.Size(101, 25);
            this.cb_DbType.TabIndex = 0;
            this.cb_DbType.SelectedIndexChanged += new System.EventHandler(this.cb_DbType_Changed);
            // 
            // lbl_db
            // 
            this.lbl_db.AutoSize = true;
            this.lbl_db.Location = new System.Drawing.Point(40, 29);
            this.lbl_db.Name = "lbl_db";
            this.lbl_db.Size = new System.Drawing.Size(56, 17);
            this.lbl_db.TabIndex = 1;
            this.lbl_db.Text = "DbType:";
            // 
            // btn_Test
            // 
            this.btn_Test.Location = new System.Drawing.Point(708, 107);
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size(75, 23);
            this.btn_Test.TabIndex = 3;
            this.btn_Test.Text = "Test";
            this.btn_Test.UseVisualStyleBackColor = true;
            this.btn_Test.Click += new System.EventHandler(this.btn_Test_Click);
            // 
            // tb_ConnStr
            // 
            this.tb_ConnStr.Location = new System.Drawing.Point(40, 67);
            this.tb_ConnStr.Name = "tb_ConnStr";
            this.tb_ConnStr.PlaceholderText = "Please Choice Your DbType!";
            this.tb_ConnStr.Size = new System.Drawing.Size(748, 23);
            this.tb_ConnStr.TabIndex = 4;
            // 
            // tb_Output
            // 
            this.tb_Output.BackColor = System.Drawing.SystemColors.InfoText;
            this.tb_Output.ForeColor = System.Drawing.SystemColors.Info;
            this.tb_Output.Location = new System.Drawing.Point(16, 223);
            this.tb_Output.Multiline = true;
            this.tb_Output.Name = "tb_Output";
            this.tb_Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_Output.Size = new System.Drawing.Size(772, 307);
            this.tb_Output.TabIndex = 6;
            // 
            // btn_Stop
            // 
            this.btn_Stop.Enabled = false;
            this.btn_Stop.Location = new System.Drawing.Point(610, 136);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(75, 23);
            this.btn_Stop.TabIndex = 7;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Visible = false;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // cb_IsAck
            // 
            this.cb_IsAck.AutoSize = true;
            this.cb_IsAck.Location = new System.Drawing.Point(158, 107);
            this.cb_IsAck.Name = "cb_IsAck";
            this.cb_IsAck.Size = new System.Drawing.Size(57, 21);
            this.cb_IsAck.TabIndex = 8;
            this.cb_IsAck.Text = "isAck";
            this.cb_IsAck.UseVisualStyleBackColor = true;
            this.cb_IsAck.Visible = false;
            // 
            // tb_RoutingKey
            // 
            this.tb_RoutingKey.Location = new System.Drawing.Point(40, 136);
            this.tb_RoutingKey.Name = "tb_RoutingKey";
            this.tb_RoutingKey.PlaceholderText = "Please Enter Your RoutingKey!";
            this.tb_RoutingKey.Size = new System.Drawing.Size(493, 23);
            this.tb_RoutingKey.TabIndex = 10;
            this.tb_RoutingKey.Visible = false;
            // 
            // tb_SendMsg
            // 
            this.tb_SendMsg.Location = new System.Drawing.Point(40, 180);
            this.tb_SendMsg.Name = "tb_SendMsg";
            this.tb_SendMsg.PlaceholderText = "Please Enter Your Message!";
            this.tb_SendMsg.Size = new System.Drawing.Size(743, 23);
            this.tb_SendMsg.TabIndex = 11;
            this.tb_SendMsg.Visible = false;
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(708, 136);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(75, 23);
            this.btn_Start.TabIndex = 12;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // cb_ClearOutput
            // 
            this.cb_ClearOutput.AutoSize = true;
            this.cb_ClearOutput.Location = new System.Drawing.Point(40, 107);
            this.cb_ClearOutput.Name = "cb_ClearOutput";
            this.cb_ClearOutput.Size = new System.Drawing.Size(106, 21);
            this.cb_ClearOutput.TabIndex = 13;
            this.cb_ClearOutput.Text = "isClearOuptut";
            this.cb_ClearOutput.UseVisualStyleBackColor = true;
            // 
            // DataSourceTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 540);
            this.Controls.Add(this.cb_ClearOutput);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.tb_SendMsg);
            this.Controls.Add(this.tb_RoutingKey);
            this.Controls.Add(this.cb_IsAck);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.tb_Output);
            this.Controls.Add(this.tb_ConnStr);
            this.Controls.Add(this.btn_Test);
            this.Controls.Add(this.lbl_db);
            this.Controls.Add(this.cb_DbType);
            this.MaximizeBox = false;
            this.Name = "DataSourceTestForm";
            this.Text = "DataSourceTestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_DbType;
        private System.Windows.Forms.Label lbl_db;
        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.TextBox tb_ConnStr;
        private System.Windows.Forms.TextBox tb_Output;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.CheckBox cb_IsAck;
        private System.Windows.Forms.TextBox tb_RoutingKey;
        private System.Windows.Forms.TextBox tb_SendMsg;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.CheckBox cb_ClearOutput;
    }
}