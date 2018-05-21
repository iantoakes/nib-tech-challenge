Feature: Fulfillment
    As a warehouse officer 
    I want to be able to initiate an order run
    So that I can deliver orders according to my pending orders and inventory availability

Scenario: Order can be fulfilled
    Given I have an order awaiting fulfilment
    And the order has "Small Widget" included
    And stock is available to fulfil that order
    When I submit the fulfillment
    Then I expect the order to be processed
    And I expect that the order status is "Fulfilled"
    And I expect that product quantity in the Legacy system is updated 
