﻿GStore eCommerce builder and application host
by Prosperino "Reno" Gallipoli
renogmusic@yahoo.com

Personal Home Page
http://www.renog.info

GStore Home Page
http://www.gstore.renog.info


GStore eCommerce Site Builder and CMS Platform
Copyright (C) 2014 Prosperino Gallipoli

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

 --installing:
download the solution file, ctrl-B to build, F5 to run.
the app will build out the database structure, run migrations, and seed the database with a sample storefront, client, and sample site pages.
two users are created by the seed class;   
system administrator login:   admin@domain.com the password is: password
regular user login: name@domain.com  password is: password
verify project settings for GStore database connection to ensure the right SQL Server instance
(right-click project, properties, go to settings tab)


Technologies Highlighted:

Microsoft MVC 5, 
Entity Framework 6 Code First,
Repository Pattern with unit of work abstraction and multiple providers (also in-memory list provider for unit testing)
ASP Identity 2.0 forms cookie and token auth
WebApi 2.0 and WebApi auth tokens for Http clients
Twilio SMS API integrated with Identity and site for notification of messages
SendGrid Email with email confirmation of accounts, forgot password, reset password, and lockout notification
Dynamic bootstrap themes; some included and can be swapped out from admin section
Entity Framework Code first with POCO's; simple to add properties, fields to database structures in /Models
ASP.Net Identity 2.0; identity classes brough into App classes in /Identity for customization of role, user, user claims
JQuery, JQueryUI : more to come
eCommerce and CMS features including dynamic pages, dynamic urls, and widgets for page sections
Structured exception handling with logging to database tables: SystemEvents, SecurityEvents, PageViewEvents, FileNotFoundLog, BadRequests
Multi-customer multi-store design with multiple clients able to run in the same app or in separate virtual directories

Main Modules: 

- - - Configuration - - -
Settings application-wide are defined in Project Settings UI and transfer to web.config as strongly typed properties
settings for individual store fronts are defined in StoreFront or Client table
Database auto-creation using entity framework code first.  Models can be extended application-wide
Auto-fill database seed data in Data\SeedDataExtensions.cs
structured exception handling in Controllers and also application scope in /Exceptions folder
Settings to enable/disable  SMS and email and set up database migration strategy
Enable/disable two-factor authentication for new users
Extensive logging to database for security events, system exceptions, and page views.


- - - Account module - - -
Account setup with ASP.Net Identity 2.0 including
registration to site using email address or external providers (facebook, google, twitter, microsoft account)
email confirmation/verification
two factor authentication (phone and email options)
forgot password, password reset, and two-factor verification (if set in app settings)
OWIN tokens for client API's at /token API
SignalR broadcast to all current users when a new user signs up; setting to enable/disable: IdentityEnableNewUserRegisteredBroadcast
Users sign in with email, but through the rest of the system they are known by their full name, not a generic user id (for privacy)


- - - Manage Module - - -
User manager for site settings, notifications by email and sms when there is a message for them from the site, account locked out, etc.
ability to link a phone number for sms-notifications of site messages
change password, add phone, confirm email, and notification preferences


- - - Notifications Module - - -
Site messaging system to automatically welcome a new user to the site
Notifications sent to verified email address and sms to phone (if user enables it)
send messages to other public users or system admins through the site.
send links within site and outside of site (internal/external) in notifications
... more to come: public profiles, forums, notifications for product reviews, comments, product inquiries


- - CMS Module - -
dynamic page controller with Bootstrap themes, and customizations by storefront (css)
pages may be previewed in pending state (as they're customized), or scheduled for specific start/end dates of activtity
multiple versions of pages are used in scenarios of timed sales, specific deadlines, and expiration of offers
dynamic url routing and page-specific meta tags customized by page; good for SEO and permalinks, doorway pages
Responsive fluid templates fit and WORK on all devices.  Auto-adapt to mobile and tablet with menu transformations.
... more to come: admin tools for page editing, widgets, SEO tools, optimizations for mobile and tablet and JQueryMobile


- - - Order Module - - -
... Coming soon: wish lists, shopping cart, order status and notifications, share with a friend, tracking, order history, discounts, payment gateways


- - - Store Admin Module - - -
... Coming soon: Manage Catalogs, Products, Product pages, and ordering process


work in progress, submit feedback/comments to
renogmusic@yahoo.com


