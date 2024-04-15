# Test Credit Card Numbers

## Good Numbers

 "5555555555554444" MasterCard test credit card number
 "4012888888881881" Visa test credit card number
 "3056930009020004" Diners Club test credit card number
 "3566111111111113" JCB test credit card number


## Bad Numbers

 "5558555555554444"     MasterCard test card number with single digit transcription error 5 -> 8
 "5558555555554434"     MasterCard test card number with single digit transcription error 4 -> 3
 "3059630009020004"     Diners Club test card number with two digit transposition error 69 -> 96 
 "3056930009002004"     Diners Club test card number with two digit transposition error 20 -> 02
 "5559955555554444"     MasterCard test card number with two digit twin error 55 -> 99
 "3566111144111113"     JCB test card number with two digit twin error 11 -> 44

 (from https://medium.com/@michael.harges/implementing-the-luhn-algorithm-in-c-9232c414da3a)