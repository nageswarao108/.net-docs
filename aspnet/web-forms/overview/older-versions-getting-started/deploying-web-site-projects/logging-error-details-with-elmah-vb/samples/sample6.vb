' ... Save user's information to the database ...
...
' Attempt to send the user a confirmation e-mail
	' ... Send an e-mail ...
Try
Catch e As Exception
' Error in sending e-mail. Log it!
ErrorSignal.FromCurrentContext().Raise(e)
End Try