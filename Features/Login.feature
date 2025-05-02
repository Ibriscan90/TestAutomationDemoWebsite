Feature: Login

Check if login is successful with correct username and password, as well as after login the user is redirected to the Admin panel

  Scenario: Successful login with valid credentials
    Given the user is on the login page
    When the user logs in with username and password 
    Then the user should see the admin panel
    Then the user logs out of the admin account


Scenario Outline: Unsuccessful login with invalid credentials
    Given the user is on the login page
    When the user logs in with an invalid "<username>" and "<password>" 
    Then the user should see an error message

    Examples:
  | username   | password   |
  | admin      | wrongpass  |
  | wronguser  | password   |
  | wronguser  | wrongpass  |

