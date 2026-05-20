/*
 * 
 * 
    Problem Statement: Design a parking lot system that can manage multiple parking lots, each with a certain capacity. 
    The system should allow cars to park and unpark, and it should keep track of the available spaces in each parking lot.

    Functionality Requirements:
        - Entry and Exit Gate 
        - vehicle types (e.g., car, motorcycle, truck) and corresponding parking space requirements
        - Floor management (e.g., assigning parking spaces based on vehicle type and availability)
        - Ticketing system (e.g., generating parking tickets with entry time, calculating parking fees based on duration)
        - Payment system (e.g., accepting payments for parking fees, providing change if necessary)
        - Assign parking spaces to vehicles based on their type and availability
        - Handle edge cases such as full parking lots, invalid vehicle types, and payment errors
        - Parking slot system should be designed to be scalable and maintainable, allowing for future enhancements such as integration with mobile apps for parking reservations and real-time availability updates.
        - Slot selection strategy (e.g., nearest available slot, random assignment, or a more complex algorithm based on usage patterns)

 * 
 * 
 * 
 
    Functional Requirements:
        - Should suppot multiple entry and exit gates for the parking lot
        - Should allocate parking spaces based on vehicle type and availability
        - Should generate parking tickets with entry time and calculate parking fees based on duration
        - multiple payment options (e.g., cash, credit card, mobile payment)
        - multiple vehicle types (e.g., car, motorcycle, truck) and corresponding parking space requirements
        - payment system should handle errors gracefully and provide clear feedback to users
        - multiple parking lots with different capacities should be supported, and the system should keep track of available spaces in each lot

    Non-Functional Requirements:
            - The system should be designed to be scalable and maintainable, allowing for future enhancements such as integration with mobile apps for parking reservations and real-time availability updates.
            - The system should be designed with security in mind, ensuring that sensitive information such as payment details is protected.
            - The system should be designed to handle high traffic volumes, especially during peak hours, without significant performance degradation.
            - Thread safety should be considered, especially if the system is expected to handle concurrent parking and unparking operations across multiple entry and exit gates.

    Core Entities:
        - Parking lot
        - Parking Floor
        - Parking lot
        - ticket 
        - vehical 
        - payment 
        - fee Stratergy 
        - Slot allocationt stratergy 
        - Entry gate 
        - Exit gate
        - Parking lot manager

    Relationships:
        - A parking lot can have multiple parking floors.
        - Each parking floor can have multiple parking spaces.
        - A vehicle can park in a parking space and receive a ticket.
        - The ticket contains information about the entry time and the assigned parking space.
        - When a vehicle unparks, the system calculates the parking fee based on the duration of the stay and the fee strategy.
        - The payment system processes the payment for the parking fee and provides feedback to the user.
 * 
 * 
 */


interface ISlotSelectionStrategy
{
    ParkingSpace SelectSlot(Vehicle vehicle, ParkingLot parkingLot);
}
