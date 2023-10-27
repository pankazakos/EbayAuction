# EbayAuction Software Requirements Specification

## 1. Introduction

The purpose of the EbayAuction web application is to provide a platform for users to create auctions for their personal items, as well as participate in bidding on items listed by other users. This document describes both functional and non-functional requirements of the project.


## 2. System Description

There are three main roles for the users of the web app:

1. Guests
2. Registered Users
3. Administrators

The main actions of the users are briefly described below:

- Browse and search for auctions (All users).
- Create and manage auctions for items (Registered Users).
- Place bids on items listed in auctions (Registered Users).
- Suspend or delete users and auctions (Admins).


## 3. Functional Requirements

### 3.1 Search and Browse

Users can browse and filter auctions by category, price range, and location. Auction listings should display relevant details. They can also search for specific auctions by their title or with keywords. The search will be based on full-text search.

#### User stories
- As a guest user, I want to be able to browse and search for auctions by category, price range, and location so that I can find items of interest quickly.
- As a registered user, I want to search for specific auctions by title or keywords to find items that match my interests.


### 3.2 User Registration and Authentication

Users can create a new account by providing a unique username, email, password, and phone number. After successfully creating the account, they can log in using their username and password.

#### User stories
- As a new user, I want to create an account by providing a unique username, email, password, and phone number so that I can start using EbayAuction.
- As a registered user, I want to log in using my username and password to access my account and participate in auctions.


### 3.3 Auction Management

#### 3.3.1 Create Auction

Users can create new auctions by providing a title, item description and starting bid price. Optionally, they can include a maximum bid and upload images of the item. Once created, the user will be able to start the auction by activating it and making it public while selecting a specific datetime for expiration.

#### 3.3.2 Edit/Delete Auction

Users can edit or delete their own auctions before publishing them. Editing includes changing the item description, bid price, and auction expiration datetime.

#### User stories
- As a registered user, I want to create new auctions by providing a title, item description and starting bid price to list my items for bidding.
- As a registered user, I want the option to define a maximum bid or buying price and upload images of the item I'm auctioning to showcase it to potential bidders.
- As a registered user, I want to be able to edit my auctions before publishing them. This includes changing the item title, description and bid price.
- As a registered user, I want the ability to delete my own auctions before they are published if I decide not to proceed with the listing.
- As a registered user, I want to set a specific datetime for my auction expiry, once I make it public.

### 3.4 Bidding

Users can place bids on active auctions. Bids must be higher than the current highest bid and adhere to auction rules. The minimum bid increment for each item depends on the current amount of winning bid. Bid increments are smaller when the bid price is low and larger in higher price brackets.

| Current Price Range    | Bid Increment |
|------------------------|---------------|
| $0.01–$0.99            | $0.05         |
| $1.00–$4.99            | $0.25         |
| $5.00–$24.99           | $0.50         |
| $25.00–$99.99          | $1.00         |
| $100.00–$249.99        | $2.50         |
| $250.00–$499.99        | $5.00         |
| $500.00–$999.99        | $10.00        |
| $1000.00–$2499.99      | $25.00        |
| $2500.00–$4999.99      | $50.00        |
| $5000.00 and up        | $100.00       |


#### User stories
- As a registered user, I want to place bids on active auctions in order to be able to win an auction.


### 3.5 Winner Notification

Winning a bid results in a notification to the user. The winner will also get access to the email and phone number of the seller in order to communicate and schedule the item pickup or shipping.

#### User stories
- As a registered user, I want to receive notifications when I win an auction so that I can be informed about my successful bids.
- As a winning bidder, I want to access the email and phone number of the seller so that I can communicate with them and arrange for item pickup or shipping.

### 3.6 Rating
The user will be able to leave a review on a 5-star scale and provide text describing their experience with the seller. Users can view the seller's ratings by clicking on their name via the auction post.

#### User stories
- As a winning bidder, I want to leave a review with a 5-star rating and text describing my experience with the seller to provide feedback to the community.
- As a user, I want to view a seller's ratings by clicking on their name via the auction post to assess their trustworthiness.


### 3.7 User Account Management

Users can view their own profiles, showing their account details, auction, and bid history.

#### User stories
- As a user, I want to view my profile to see my account details, including my username, email, and auction/bid history.


## 4. Non-Functional Requirements

### 4.1 Security
- **Password Storage:** User passwords should be securely stored using hashing and salting. This ensures that passwords are protected against unauthorized access and potential breaches.
- **JWT Authentication:** The application should utilize JSON Web Tokens (JWT) for user authentication. In addition to access tokens, the system will implement refresh tokens to enhance security.
- **Administrative Access:** Initially, the application should have one preconfigured administrative user account. The admin account will be used for user management, and other administrative tasks.

### 4.2 Error Handling
- **Error Messaging:** The system should provide clear and user-friendly error messages in case of incorrect user inputs or system failures. Error messages should be informative and guide users to resolve issues effectively.
- **Logging:** The application should maintain detailed error logs on the console, including timestamps and error descriptions, to aid in troubleshooting and debugging.

### 4.3 Testing
- **Integration Testing:** Integration tests should be added to verify the interactions and compatibility of various system components.
- **Unit Testing:** Unit tests will be considered to get added to validate the desired behavior of individual classes and methods. The decision to include unit testing will be finalized during the development phase.

### 4.4 Usability
- **Responsive Design:** The application design should be responsive to ensure a consistent and user-friendly experience across various devices and screen sizes, including mobile devices and desktop computers.


## 5. Assumptions
Since EbayAuction is a demo web application, there are various missing features compared to an application targeting real users. Here are some of the assumptions made for the current project:

- **Non-Payment:** Payment processing and financial transactions, are intentionally omitted. This decision is made to simplify the application's scope and focus on core functionalities.
- **Security Measures:** Security measures will be taken such as password hashing and salting, as well as JWT-based authentication. While these measures enhance security, the application may not meet the same strict security standards as a production application.
- **Limited Elevated Privileges:** While the application will include one initial administrative user for system management, his administrative capabilities are limited compared to those of a production ready application. Administrative functions are primarily focused on user management.


